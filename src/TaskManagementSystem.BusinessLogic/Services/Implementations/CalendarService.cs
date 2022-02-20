using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Dal.Repositories.Implementations;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarService
{
    private readonly IUnitOfWork unitOfWork;
    private readonly IUserRepository userRepository;
    private readonly CalendarRepository calendarRepository;
    private readonly CalendarParticipantRepository calendarParticipantRepository;

    public CalendarService(IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        CalendarRepository calendarRepository, 
        CalendarParticipantRepository calendarParticipantRepository)
    {
        this.unitOfWork = unitOfWork;
        this.userRepository = userRepository;
        this.calendarRepository = calendarRepository;
        this.calendarParticipantRepository = calendarParticipantRepository;
    }

    public async Task<ISet<Calendar>> GetUserCalendars(Guid userId)
    {
        return await calendarRepository.GetByUserId(userId);
    }

    public async Task<Calendar> CreateCalendarAsync(CreateCalendarData data)
    {
        data.AssertNotNull();

        // Проверка на существование с таким же именем.
        Calendar? existed = await calendarRepository.GetByNameAsync(data.Name);
        if (existed is not null)
        {
            throw new BusinessLogicException("Календарь с таким именем уже существует");
        }

        unitOfWork.BeginTransaction();
        
        Calendar calendar = new(Guid.NewGuid(), data.Name, data.Description, DateTime.UtcNow);
        await calendarRepository.InsertAsync(calendar);

        CalendarParticipant owner = new(Guid.NewGuid(),calendar.Id, data.CreatorId, DateTime.UtcNow, CalendarRole.Creator);
        await calendarParticipantRepository.InsertAsync(owner);
        
        unitOfWork.CommitTransaction();
        return calendar;
    }

    public async Task<Calendar> EditCalendarAsync(CalendarEditData data)
    {
        data.AssertNotNull();
        
        CalendarWithParticipants calendarInfo = await GetCalendarInfoAsync(data.CalendarId);
        
        //Проверка, что редактирует администратор или создатель календаря
        CheckIsAdminOrCreator(data.EditorId, calendarInfo.Participants);

        string name = calendarInfo.Calendar.Name;
        string description = calendarInfo.Calendar.Description;
        if (!string.IsNullOrWhiteSpace(data.Name))
        {
            name = data.Name;
        }
        if (!string.IsNullOrWhiteSpace(data.Description))
        {
            description = data.Description;
        }

        Calendar updatedCalendar = new(calendarInfo.Calendar.Id, name, description, calendarInfo.Calendar.CreationDateUtc);
        await calendarRepository.UpdateAsync(updatedCalendar);

        return updatedCalendar;
    }

    public async Task<CalendarWithParticipants> AddParticipantsAsync(AddCalendarParticipantsData data)
    {
        data.AssertNotNull();
        
        if(!data.Users.All(x=> x.Role is CalendarRole.Admin or CalendarRole.Participant))
        {
            //TODO: Выводить конкретный список некорректных ролей
            throw new BusinessLogicException("Обнаружена некорректная роль");
        }
        
        CalendarWithParticipants calendarInfo = await GetCalendarInfoAsync(data.CalendarId);

        //Проверка, что редактирует администратор или создатель календаря
        CheckIsAdminOrCreator(data.AdderId, calendarInfo.Participants);
        
        var userIds = data.Users.Select(x => x.UserId).ToHashSet();

        //Проверка, что какие-то пользователи уже могут быть в календаре
        if (userIds.Intersect(calendarInfo.Participants.Select(x => x.UserId)).Any())
        {
            //TODO: Выводить конкретный список пользователей
            throw new BusinessLogicException("Некоторые пользователи уже добавлены в календарь");
        }
        
        //Проверка, что добавляемые пользователи вообще существуют
        var users = await userRepository.GetByIdsAsync(userIds);
        if (users.Count != userIds.Count)
        {
            //TODO: Выводить конкретный список пользователей
            throw new BusinessLogicException("Найден не полный список участников");
        }

        //TODO: Баг, что удаленный и заново добавленный пользователь будет иметь разные Id
        var participantsToAdd = data.Users.Select(x => 
            new CalendarParticipant(Guid.NewGuid(), calendarInfo.Calendar.Id, x.UserId, DateTime.UtcNow, x.Role)).ToHashSet();
        await calendarParticipantRepository.InsertAllAsync(participantsToAdd);
        
        var newParticipants = await calendarParticipantRepository.GetByCalendarIdAsync(calendarInfo.Calendar.Id);
        
        return new CalendarWithParticipants(calendarInfo.Calendar, newParticipants);
    }

    public async Task<CalendarWithParticipants> DeleteParticipantsAsync(DeleteParticipantsData data)
    {
        data.AssertNotNull();
        
        CalendarWithParticipants calendarInfo = await GetCalendarInfoAsync(data.CalendarId);

        //Проверка, что редактирует администратор или создатель календаря
        CheckIsAdminOrCreator(data.RemoverId, calendarInfo.Participants);
        
        //Проверка, что удаляемые участники принадлежат этому календарю
        if (data.ParticipantsIds.Except(calendarInfo.Participants.Select(x => x.Id)).Any())
        {
            //TODO: Выводить конкретный список пользователей
            throw new BusinessLogicException("Не все пользователи принадлежат этому календарю");
        }

        //Проверка, что нельзя удалить создателя календаря
        CalendarParticipant creator = calendarInfo.Participants.Single(x => x.Role == CalendarRole.Creator);
        if (data.ParticipantsIds.Contains(creator.Id))
        {
            throw new BusinessLogicException("Нельзя удалять создателя календаря");
        }
        
        await calendarParticipantRepository.DeleteByIds(data.ParticipantsIds);
        
        var newParticipants = await calendarParticipantRepository.GetByCalendarIdAsync(calendarInfo.Calendar.Id);
        return new CalendarWithParticipants(calendarInfo.Calendar, newParticipants);
    }
    
    public async Task<CalendarWithParticipants> GetCalendarInfoAsync(Guid id)
    {
        Calendar? calendar = await calendarRepository.GetByIdAsync(id);
        if (calendar is null)
        {
            throw new BusinessLogicException($"Календарь с таким Id '{id}' не найден.");
        }

        var participants = await calendarParticipantRepository.GetByCalendarIdAsync(id);

        return new CalendarWithParticipants(calendar, participants);
    }

    private static void CheckIsAdminOrCreator(Guid userId, IEnumerable<CalendarParticipant> participants)
    {
        CalendarParticipant? participant = participants.FirstOrDefault(x => x.UserId == userId);
        
        if (participant is null)
        {
            throw new BusinessLogicException("Пользователь не является участником календаря");
        }
        
        if (participant.Role is not (CalendarRole.Admin or CalendarRole.Creator))
        {
            throw new BusinessLogicException("Пользователь не является создателем или администратором календаря");
        }
    }
}