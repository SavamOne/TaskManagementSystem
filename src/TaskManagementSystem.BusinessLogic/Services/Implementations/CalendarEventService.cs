using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Extensions;
using TaskManagementSystem.BusinessLogic.Helpers;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Resources;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Dal.Extensions;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarEventService : ICalendarEventService
{
	private readonly ICalendarParticipantRepository calendarParticipantRepository;
	private readonly ICalendarEventParticipantRepository eventParticipantRepository;
	private readonly ICalendarEventRepository eventRepository;
	private readonly IRecurrentSettingsRepository recurrentSettingsRepository;
	private readonly IUnitOfWork unitOfWork;

	public CalendarEventService(IUnitOfWork unitOfWork,
		ICalendarParticipantRepository calendarParticipantRepository,
		ICalendarEventRepository eventRepository,
		ICalendarEventParticipantRepository eventParticipantRepository,
		IRecurrentSettingsRepository recurrentSettingsRepository)
	{
		this.unitOfWork = unitOfWork;
		this.calendarParticipantRepository = calendarParticipantRepository;
		this.eventRepository = eventRepository;
		this.eventParticipantRepository = eventParticipantRepository;
		this.recurrentSettingsRepository = recurrentSettingsRepository;
	}

	public async Task<CalendarEvent> CreateEventAsync(AddCalendarEventData data)
	{
		data.AssertNotNull();

		CalendarParticipant? participant = await calendarParticipantRepository.GetByUserAndCalendarId(data.UserId, data.CalendarId);
		if (participant is null)
		{
			throw new BusinessLogicException("Пользователя и/или Календаря с таким Id не существует или этот пользователь не участвует в этом календаре.");
		}

		if (data.StartTime >= data.EndTime)
		{
			throw new BusinessLogicException("Дата окончания события меньше, чем дата начала");
		}

		if (DateTime.UtcNow - data.StartTime.UtcDateTime > TimeSpan.FromDays(1))
		{
			throw new BusinessLogicException("Нельзя создавать события, начавшиеся более 24 часов назад");
		}

		Guid eventId = Guid.NewGuid();
		RecurrentEventSettings? recurrentSettings = data.RecurrentSettingsData is not null
			? ValidateAndCreateRecurrentSettings(data.RecurrentSettingsData, eventId, data.EndTime.UtcDateTime)
			: null;

		CalendarEvent calendarEvent = new(eventId,
			data.CalendarId,
			data.Name,
			data.Description,
			data.EventType,
			data.Place,
			data.StartTime.UtcDateTime.StripSeconds(),
			data.EndTime.UtcDateTime.StripSeconds(),
			data.IsPrivate,
			DateTime.UtcNow,
			recurrentSettings is not null);

		CalendarEventParticipant calendarEventParticipant = new(Guid.NewGuid(),
			eventId,
			participant.Id,
			CalendarEventParticipantRole.Creator,
			CalendarEventParticipantState.Confirmed);

		unitOfWork.BeginTransaction();
		await eventRepository.InsertAsync(calendarEvent);
		await eventParticipantRepository.InsertAsync(calendarEventParticipant);

		if (recurrentSettings is not null)
		{
			await recurrentSettingsRepository.InsertForEvent(recurrentSettings);
		}

		unitOfWork.CommitTransaction();
		return calendarEvent;
	}

	public async Task<ICollection<CalendarEvent>> GetCalendarEventsInPeriodAsync(GetCalendarEventsInPeriodData data)
	{
		data.AssertNotNull();

		if (data.StartPeriod - data.EndPeriod >= TimeSpan.FromDays(60))
		{
			throw new BusinessLogicException(LocalizedResources.DaysLimitOutOfRange, 60);
		}

		CalendarParticipant? participant = await calendarParticipantRepository.GetByUserAndCalendarId(data.UserId, data.CalendarId);
		if (participant is null)
		{
			throw new BusinessLogicException("Пользователя и/или Календаря с таким Id не существует или этот пользователь не участвует в этом календаре.");
		}

		var events = await eventRepository.GetStandardCalendarEventsInRange(data.CalendarId, data.StartPeriod.UtcDateTime, data.EndPeriod.UtcDateTime);
		var repeatedEvents = await eventRepository.GetRepeatedCalendarEvents(data.CalendarId);

		if (!participant.IsAdminOrCreator())
		{
			foreach (CalendarEvent @event in events.Union(repeatedEvents).Where(x => x.IsPrivate))
			{
				if (!await eventParticipantRepository.ContainsCalendarParticipantInEvent(participant.Id, @event.Id))
				{
					HideEventInfo(@event);
				}
			}
		}

		return await CalculateEventsInPeriodAsync(repeatedEvents, events, data.StartPeriod.UtcDateTime, data.EndPeriod.UtcDateTime);
	}

	public async Task<ICollection<CalendarEvent>> GetEventsForUserInPeriodAsync(GetEventsInPeriodForUserData data)
	{
		data.AssertNotNull();

		if (data.StartPeriod - data.EndPeriod >= TimeSpan.FromDays(60))
		{
			throw new BusinessLogicException(LocalizedResources.DaysLimitOutOfRange, 60);
		}

		//TODO: Существование Id пользователя.

		var events = await eventRepository.GetStandardEventsInRangeForUser(data.UserId, data.StartPeriod.UtcDateTime, data.EndPeriod.UtcDateTime);
		var repeatedEvents = await eventRepository.GetRepeatedEventsForUser(data.UserId);

		return await CalculateEventsInPeriodAsync(repeatedEvents, events, data.StartPeriod.UtcDateTime, data.EndPeriod.UtcDateTime);
	}

	public async Task DeleteEventAsync(DeleteEventData data)
	{
		CalendarEventParticipant? eventParticipant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);

		if (eventParticipant is not null)
		{
			if (!eventParticipant.IsCreator() && !eventParticipant.CalendarParticipant.IsAdminOrCreator())
			{
				throw new BusinessLogicException("Только создатель события или администратор календаря может удалять события.");
			}
		}
		else
		{
			//TODO: Оптимизировать
			CalendarEvent? @event = await eventRepository.GetById(data.EventId);
			if (@event is null)
			{
				throw new BusinessLogicException("События с таким Id не существует");
			}

			CalendarParticipant? calendarParticipant = await calendarParticipantRepository.GetByUserAndCalendarId(data.UserId, @event.CalendarId);
			if (calendarParticipant is null)
			{
				throw new BusinessLogicException("Этого пользователя нет в календаре");
			}

			if (!calendarParticipant.IsAdminOrCreator())
			{
				throw new BusinessLogicException("Только создатель события или администратор календаря может удалять события.");
			}
		}

		await eventRepository.DeleteByIdAsync(data.EventId);
	}

	public async Task<CalendarEvent> EditEventAsync(EditCalendarEventData data)
	{
		CalendarEventParticipant? participant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
		if (participant is null)
		{
			throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
		}
		if (!participant.IsParticipantOrCreator())
		{
			throw new BusinessLogicException("Изменять событие может только создатель или участники события.");
		}
		if (data.IsRepeated && data.RecurrentSettingsData is null)
		{
			throw new BusinessLogicException("Отсутствуют настройки для повторения.");
		}

		CalendarEvent @event = ( await eventRepository.GetById(data.EventId) )!;

		@event.Name = data.Name ?? @event.Name;
		@event.Description = data.Description ?? @event.Description;
		@event.Place = data.Place ?? @event.Place;
		@event.EventType = data.EventType ?? @event.EventType;
		@event.IsPrivate = data.IsPrivate ?? @event.IsPrivate;
		@event.StartTimeUtc = data.StartTime?.UtcDateTime.StripSeconds() ?? @event.StartTimeUtc;
		@event.EndTimeUtc = data.EndTime?.UtcDateTime.StripSeconds() ?? @event.EndTimeUtc;
		@event.IsRepeated = data.IsRepeated;

		if (@event.StartTimeUtc >= @event.EndTimeUtc)
		{
			throw new BusinessLogicException("Дата окончания события меньше, чем дата начала");
		}

		RecurrentEventSettings? recurrentSettings = data.IsRepeated
			? ValidateAndCreateRecurrentSettings(data.RecurrentSettingsData!, @event.Id, @event.EndTimeUtc)
			: null;

		unitOfWork.BeginTransaction();
		await eventRepository.UpdateAsync(@event);
		await recurrentSettingsRepository.DeleteForEvent(@event.Id);

		if (data.IsRepeated)
		{
			await recurrentSettingsRepository.InsertForEvent(recurrentSettings!);
		}

		unitOfWork.CommitTransaction();
		return @event;
	}

	public async Task<CalendarEventWithParticipants> AddEventParticipant(AddEventParticipantsData data)
	{
		// Проверка, что добавляем только участников или информируемых
		if (!data.CalendarParticipants.All(x => x.Role.IsParticipantOrInform()))
		{
			throw new BusinessLogicException("Можно добавлять только участников или информируемых.");
		}

		CalendarEventWithParticipants info = await GetEventInfo(data.UserId, data.EventId);
		if (!info.CanUserEditParticipants)
		{
			throw new BusinessLogicException("Добавлять участников может только создатель или участник события.");
		}

		var calendarParticipantIds = data.CalendarParticipants.Select(x => x.CalendarParticipantId).ToHashSet();
		if (calendarParticipantIds.Intersect(info.Participants.Select(x => x.CalendarParticipantId)).Any())
		{
			throw new BusinessLogicException("Некоторые пользователи уже являются участником события");
		}

		var calendarParticipants = await calendarParticipantRepository.GetByIdsAsync(calendarParticipantIds);
		if (calendarParticipants.Count != calendarParticipantIds.Count)
		{
			throw new BusinessLogicException("Не все идентификаторы присутствуют в календаре");
		}
		if (calendarParticipants.Any(x => x.CalendarId != info.Event.CalendarId))
		{
			throw new BusinessLogicException("Не все пользователи принадлежат календарю");
		}

		var participantsToAdd = data.CalendarParticipants
		   .Select(x =>
			{
				return new CalendarEventParticipant(Guid.NewGuid(),
					info.Event.Id,
					x.CalendarParticipantId,
					x.Role,
					CalendarEventParticipantState.Unknown);
			})
		   .ToList();

		await eventParticipantRepository.InsertAllAsync(participantsToAdd);

		return await GetEventInfo(data.UserId, data.EventId);
	}

	public async Task<CalendarEventWithParticipants> EditEventParticipants(EditEventParticipantsData data)
	{
		if (!data.EventParticipants.All(x => x.Role?.IsParticipantOrInform() ?? x.Delete))
		{
			//TODO: Выводить конкретный список некорректных ролей
			throw new BusinessLogicException("Обнаружена некорректная роль");
		}

		CalendarEventWithParticipants info = await GetEventInfo(data.UserId, data.EventId);
		if (!info.CanUserEditParticipants)
		{
			throw new BusinessLogicException("Изменять участников может только создатель или участник события.");
		}

		var toChange = new List<CalendarEventParticipant>();
		var toDelete = new HashSet<Guid>();

		foreach (EditEventParticipantData changeEventParticipantData in data.EventParticipants)
		{
			CalendarEventParticipant? participant = info.Participants
			   .FirstOrDefault(x => x.Id == changeEventParticipantData.EventParticipantId);
			if (participant is null)
			{
				throw new BusinessLogicException($"Участника с id {changeEventParticipantData.EventParticipantId} не найдено");
			}

			if (participant.CalendarParticipant!.UserId == data.UserId)
			{
				throw new BusinessLogicException("Нельзя изменить роль/удалить самого себя");
			}

			if (participant.Role.IsCreator())
			{
				throw new BusinessLogicException("Нельзя изменить роль создателя события");
			}

			if (!changeEventParticipantData.Delete)
			{
				participant.Role = changeEventParticipantData.Role!.Value;
				toChange.Add(participant);
			}
			else
			{
				toDelete.Add(participant.Id);
			}
		}

		unitOfWork.BeginTransaction();
		await eventParticipantRepository.UpdateAllAsync(toChange);
		await eventParticipantRepository.DeleteByIdsAsync(toDelete);
		unitOfWork.CommitTransaction();

		return await GetEventInfo(data.UserId, data.EventId);
	}

	public async Task<CalendarEventWithParticipants> GetEventInfo(GetEventInfoData data)
	{
		CalendarEventWithParticipants eventInfo = await GetEventInfo(data.UserId, data.EventId);

		CalendarEventParticipant? eventParticipant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
		// HACK: CanUserDeleteEvent == Админ или Создатель календаря
		if (!eventInfo.Event.IsPrivate || eventParticipant != null || eventInfo.CanUserDeleteEvent)
		{
			return eventInfo;
		}

		HideEventInfo(eventInfo.Event);
		return new CalendarEventWithParticipants(eventInfo.Event,
			Enumerable.Empty<CalendarEventParticipant>(),
			eventInfo.CanUserEditEvent,
			eventInfo.CanUserEditParticipants,
			eventInfo.CanUserDeleteEvent,
			eventInfo.ParticipationState,
			eventInfo.NotifyBefore,
			eventInfo.RecurrentEventSettings);
	}

	public async Task<CalendarEventWithParticipants> EditParticipationState(EditParticipationStateData data)
	{
		data.AssertNotNull();

		CalendarEventParticipant? eventParticipant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
		if (eventParticipant is null)
		{
			throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
		}

		if (!eventParticipant.IsParticipantOrCreator())
		{
			throw new BusinessLogicException("Обновлять состояние участия могут только участники события.");
		}

		if (!Enum.GetValues<CalendarEventParticipantState>().Contains(data.State))
		{
			throw new BusinessLogicException("Некорректное состояние");
		}

		if (data.State is not CalendarEventParticipantState.Rejected && data.NotifyBefore is null)
		{
			throw new BusinessLogicException("Не задано время напоминания.");
		}

		if (data.State is not CalendarEventParticipantState.Rejected && data.NotifyBefore < TimeSpan.Zero && data.NotifyBefore > TimeSpan.FromDays(7))
		{
			throw new BusinessLogicException("Время напоминания должно быть в периоде от 0 секунд до 7 дней.");
		}

		eventParticipant.State = data.State;
		eventParticipant.NotifyBefore = data.State is not CalendarEventParticipantState.Rejected ? data.NotifyBefore!.Value : TimeSpan.Zero;

		await eventParticipantRepository.UpdateAllAsync(new[]
		{
			eventParticipant
		});

		return await GetEventInfo(data.UserId, data.EventId);
	}

	private static void HideEventInfo(CalendarEvent @event)
	{
		@event.Name = "[Приватное событие]";
		@event.Description = "[Приватное событие]";
		@event.Place = "[Приватное событие]";
		@event.EventType = EventType.Unknown;
	}

	private static RecurrentEventSettings ValidateAndCreateRecurrentSettings(AddRecurrentSettingsData recurrentSettingsData, Guid eventId, DateTime eventEndTimeUtc)
	{
		if (!Enum.GetValues<RepeatType>().Contains(recurrentSettingsData.RepeatType))
		{
			throw new BusinessLogicException("Вид повторения некорректный.");
		}
		if (recurrentSettingsData.RepeatType is RepeatType.OnWeekDays
		 && recurrentSettingsData.DayOfWeeks is not
			{
				Count: > 0
			})
		{
			throw new BusinessLogicException("Не заданы дни недели для повторения.");
		}

		if (recurrentSettingsData.Count == 0)
		{
			throw new BusinessLogicException("Количество повторений равно нулю.");
		}

		if (recurrentSettingsData.Until.HasValue && recurrentSettingsData.Until.Value.UtcDateTime < eventEndTimeUtc)
		{
			throw new BusinessLogicException("Окончание повторения меньше, чем дата окончания события.");
		}

		return new RecurrentEventSettings(eventId,
			recurrentSettingsData.RepeatType,
			recurrentSettingsData.Until?.UtcDateTime,
			recurrentSettingsData.Count,
			recurrentSettingsData.DayOfWeeks);
	}

	private async Task<ICollection<CalendarEvent>> CalculateEventsInPeriodAsync(ICollection<CalendarEvent> repeatedEvents,
		ICollection<CalendarEvent> events,
		DateTime startPeriodUtc,
		DateTime endPeriodUtc)
	{
		var repeatedEventsIds = repeatedEvents.Select(x => x.Id).ToHashSet();
		var recurrentSettings = ( await recurrentSettingsRepository.GetForEvents(repeatedEventsIds) ).ToDictionary(x => x.EventId);

		List<CalendarEvent> finalRepresentation = new(events);
		foreach (CalendarEvent repeatedEvent in repeatedEvents.AsParallel())
		{
			var calculated = RecurrenceCalculator.Calculate(repeatedEvent, recurrentSettings[repeatedEvent.Id], startPeriodUtc, endPeriodUtc);
			finalRepresentation.AddRange(calculated);
		}

		return finalRepresentation;
	}

	private async Task<CalendarEventWithParticipants> GetEventInfo(Guid userId, Guid eventId)
	{
		CalendarEvent? @event = await eventRepository.GetById(eventId);
		if (@event is null)
		{
			throw new BusinessLogicException("События c таким Id не существует");
		}

		CalendarParticipant? calendarParticipant = await calendarParticipantRepository.GetByUserAndCalendarId(userId, @event.CalendarId);
		if (calendarParticipant is null)
		{
			throw new BusinessLogicException("Этого пользователя нет в календаре");
		}

		var participants = await eventParticipantRepository.GetByEventId(eventId);

		CalendarEventParticipant? userIdParticipant = participants.FirstOrDefault(x => x.CalendarParticipant!.User!.Id == userId);
		bool canUserEditEvent = userIdParticipant?.IsParticipantOrCreator() ?? false;
		bool canUserEditParticipants = userIdParticipant?.IsCreator() ?? false;
		bool canUserDeleteEvent = userIdParticipant.IsCreator() || calendarParticipant.IsAdminOrCreator();
		var state = userIdParticipant?.State;
		var notifyBefore = canUserEditParticipants ? userIdParticipant?.NotifyBefore : null;

		RecurrentEventSettings? recurrentEventSettings = await recurrentSettingsRepository.GetForEvent(@event.Id);

		return new CalendarEventWithParticipants(@event,
			participants,
			canUserEditEvent,
			canUserEditParticipants,
			canUserDeleteEvent,
			state,
			notifyBefore,
			recurrentEventSettings);
	}
}