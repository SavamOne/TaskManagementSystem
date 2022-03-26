using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;
using TaskManagementSystem.BusinessLogic.Extensions;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Resources;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarEventService : ICalendarEventService
{
	private readonly ICalendarParticipantRepository calendarParticipantRepository;
	private readonly CalendarEventParticipantRepository eventParticipantRepository;
	private readonly CalendarEventRepository eventRepository;
	private readonly IUnitOfWork unitOfWork;

	public CalendarEventService(IUnitOfWork unitOfWork,
		ICalendarParticipantRepository calendarParticipantRepository,
		CalendarEventRepository eventRepository,
		CalendarEventParticipantRepository eventParticipantRepository)
	{
		this.unitOfWork = unitOfWork;
		this.calendarParticipantRepository = calendarParticipantRepository;
		this.eventRepository = eventRepository;
		this.eventParticipantRepository = eventParticipantRepository;
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

		if (DateTimeOffset.Now - data.StartTime.ToLocalTime() > TimeSpan.FromDays(1))
		{
			throw new BusinessLogicException("Нельзя создавать события, начавшиеся более 24 часов назад");
		}

		CalendarEvent calendarEvent = new(Guid.NewGuid(),
			data.CalendarId,
			data.Name,
			data.Description,
			data.EventType,
			data.Place,
			data.StartTime.UtcDateTime,
			data.EndTime?.UtcDateTime,
			data.IsPrivate,
			DateTime.UtcNow);
		CalendarEventParticipant calendarEventParticipant = new(Guid.NewGuid(), calendarEvent.Id, participant.Id, CalendarEventParticipantRole.Creator);

		unitOfWork.BeginTransaction();
		await eventRepository.InsertAsync(calendarEvent);
		await eventParticipantRepository.InsertAsync(calendarEventParticipant);

		unitOfWork.CommitTransaction();
		return calendarEvent;
	}

	public async Task<ICollection<CalendarEvent>> GetEventsInPeriodAsync(GetEventsInPeriodData data)
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

		var events = await eventRepository.GetStandardEventsInRange(data.CalendarId, data.StartPeriod.UtcDateTime, data.EndPeriod.UtcDateTime);

		if (participant.IsAdminOrCreator())
		{
			return events;
		}

		foreach (CalendarEvent @event in events.Where(x => x.IsPrivate))
		{
			if (!await eventParticipantRepository.ContainsCalendarParticipantInEvent(participant.Id, @event.Id))
			{
				HideEventInfo(@event);
			}
		}

		return events;
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

	public async Task<CalendarEvent> ChangeEventAsync(ChangeCalendarEventData data)
	{
		CalendarEventParticipant? participant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
		if (participant is null)
		{
			throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
		}
		if (!participant.IsCreator())
		{
			throw new BusinessLogicException("Изменять событие может только создатель события.");
		}

		CalendarEvent @event = ( await eventRepository.GetById(data.EventId) )!;

		@event.Name = data.Name ?? @event.Name;
		@event.Description = data.Description ?? @event.Description;
		@event.Place = data.Place ?? @event.Place;
		@event.EventType = data.EventType ?? @event.EventType;
		@event.IsPrivate = data.IsPrivate ?? @event.IsPrivate;
		@event.StartTimeUtc = data.StartTime?.UtcDateTime ?? @event.StartTimeUtc;
		@event.EndTimeUtc = data.EndTime?.UtcDateTime ?? @event.EndTimeUtc;

		if (@event.StartTimeUtc >= @event.EndTimeUtc)
		{
			throw new BusinessLogicException("Дата окончания события меньше, чем дата начала");
		}

		if (DateTime.UtcNow - @event.StartTimeUtc > TimeSpan.FromDays(1))
		{
			throw new BusinessLogicException("Нельзя создавать события, начавшиеся более 24 часов назад");
		}

		await eventRepository.UpdateAsync(@event);

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

		var participantsToAdd = data.CalendarParticipants.Select(x => new CalendarEventParticipant(Guid.NewGuid(), info.Event.Id, x.CalendarParticipantId, x.Role)).ToList();

		await eventParticipantRepository.InsertAllAsync(participantsToAdd);

		return await GetEventInfo(data.UserId, data.EventId);
	}

	public async Task<CalendarEventWithParticipants> ChangeEventParticipants(ChangeEventParticipantsData data)
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

		foreach (ChangeEventParticipantData changeEventParticipantData in data.EventParticipants)
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
			eventInfo.CanUserDeleteEvent);
	}

	private static void HideEventInfo(CalendarEvent @event)
	{
		@event.Name = "[Приватное событие]";
		@event.Description = "[Приватное событие]";
		@event.Place = "[Приватное событие]";
		@event.EventType = EventType.Unknown;
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
		bool canUserEditEvent = userIdParticipant?.IsCreator() ?? false;
		bool canUserEditParticipants = userIdParticipant?.IsParticipantOrCreator() ?? false;
		bool canUserDeleteEvent = calendarParticipant.IsAdminOrCreator();

		return new CalendarEventWithParticipants(@event,
			participants,
			canUserEditEvent,
			canUserEditParticipants,
			canUserDeleteEvent);
	}
}