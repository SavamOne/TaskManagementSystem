using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.BusinessLogic.Resources;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarEventService : ICalendarEventService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly ICalendarParticipantRepository calendarParticipantRepository;
    private readonly CalendarEventRepository eventRepository;
    private readonly CalendarEventParticipantRepository eventParticipantRepository;

    public CalendarEventService(
        IUnitOfWork unitOfWork,
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

        if (data.StartTime.ToLocalTime() - DateTimeOffset.Now > TimeSpan.FromDays(1))
        {
            throw new BusinessLogicException("Нельзя создавать события, начавшиеся более 24 часов назад");
        }

        CalendarEvent calendarEvent = new (Guid.NewGuid(), data.CalendarId, data.Name, data.Description, data.EventType, data.Place, data.StartTime.UtcDateTime, data.EndTime?.UtcDateTime, data.IsPrivate, DateTime.UtcNow);
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
        CalendarEventParticipant? participant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
        if (participant is null)
        {
            throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
        }
        
        if (participant.Role is not CalendarEventParticipantRole.Creator)
        {
            throw new BusinessLogicException("Только создатель календаря может удалять события.");
        }
        
        await eventRepository.DeleteByIdAsync(data.EventId);
    }
    
    public async Task<CalendarEvent> EditEventAsync(ChangeCalendarEventData data)
    {
        CalendarEventParticipant? participant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
        if (participant is null)
        {
            throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
        }
        if (participant.Role is not CalendarEventParticipantRole.Creator)
        {
            throw new BusinessLogicException("Изменять событие может только создатель события.");
        }
        
        CalendarEvent @event = (await eventRepository.GetById(data.EventId))!;

        @event.Name = data.Name ?? @event.Name;
        @event.Description = data.Description ?? @event.Description;
        @event.Place = data.Place ?? @event.Place;
        @event.EventType = data.EventType ?? @event.EventType;
        @event.IsPrivate = data.IsPrivate ?? @event.IsPrivate;
        @event.StartTimeUtc = data.StartTime?.UtcDateTime ?? @event.StartTimeUtc;
        @event.EndTimeUtc = data.EndTime?.UtcDateTime ?? @event.EndTimeUtc;

        await eventRepository.UpdateAsync(@event);

        return @event;
    }

    public async Task<CalendarEventWithParticipants> AddEventParticipant(AddEventParticipantsData data)
    {
        // Проверка, что добавляем только участников или информируемых
        if (!data.CalendarParticipants.All(x => x.Role is CalendarEventParticipantRole.Participant or CalendarEventParticipantRole.Inform))
        {
            throw new BusinessLogicException("Можно добавлять только участников или информируемых.");
        }
        
        CalendarEventParticipant? participant = await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId);
        if (participant is null)
        {
            throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
        }
        if (participant.Role is CalendarEventParticipantRole.Inform)
        {
            throw new BusinessLogicException("Добавлять события может только участник или создатель события.");
        }

        CalendarEventWithParticipants info = await GetEventInfo(data.EventId);

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
        
        return await GetEventInfo(data.EventId);
    }
    
    public async Task<CalendarEventWithParticipants> GetEventInfo(GetEventInfoData data)
    {
        CalendarEventWithParticipants eventsInfo = await GetEventInfo(data.EventId);
        
        if (eventsInfo.Event.IsPrivate && await eventParticipantRepository.GetByUserAndEventId(data.UserId, data.EventId) != null)
        {
            return eventsInfo;
        }
        
        HideEventInfo(eventsInfo.Event);
        return new CalendarEventWithParticipants(eventsInfo.Event, Enumerable.Empty<CalendarEventParticipant>());
    }

    private static void HideEventInfo(CalendarEvent @event)
    {
        @event.Name = "[Приватное событие]";
        @event.Description = "[Приватное событие]";
        @event.Place = "[Приватное событие]";
        @event.EventType = EventType.Unknown;
    }

    private async Task<CalendarEventWithParticipants> GetEventInfo(Guid eventId)
    {
        CalendarEvent? @event = await eventRepository.GetById(eventId);
        if (@event is null)
        {
            throw new BusinessLogicException("События и/или Участника события с таким Id не существует или этот пользователь не участвует в этом событии.");
        }

        var participants = await eventParticipantRepository.GetByEventId(eventId);
        return new CalendarEventWithParticipants(@event, participants);
    }
}