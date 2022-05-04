using TaskManagementSystem.BusinessLogic.Dal.Repositories;
using TaskManagementSystem.BusinessLogic.Extensions;
using TaskManagementSystem.BusinessLogic.Models.Exceptions;
using TaskManagementSystem.BusinessLogic.Models.Models;
using TaskManagementSystem.BusinessLogic.Models.Requests;
using TaskManagementSystem.Shared.Dal;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Services.Implementations;

public class CalendarService : ICalendarService
{
	private readonly ICalendarParticipantRepository calendarParticipantRepository;
	private readonly ICalendarRepository calendarRepository;
	private readonly IUnitOfWork unitOfWork;
	private readonly IUserRepository userRepository;

	public CalendarService(IUnitOfWork unitOfWork,
		IUserRepository userRepository,
		ICalendarRepository calendarRepository,
		ICalendarParticipantRepository calendarParticipantRepository)
	{
		this.unitOfWork = unitOfWork;
		this.userRepository = userRepository;
		this.calendarRepository = calendarRepository;
		this.calendarParticipantRepository = calendarParticipantRepository;
	}

	public async Task<ICollection<Calendar>> GetUserCalendars(Guid userId)
	{
		return await calendarRepository.GetByUserId(userId);
	}

	public async Task<Calendar> CreateCalendarAsync(CreateCalendarData data)
	{
		data.AssertNotNull();

		await CheckCalendarExistenceByName(data.Name);

		unitOfWork.BeginTransaction();

		Calendar calendar = new(Guid.NewGuid(), data.Name, data.Description, DateTime.UtcNow);
		await calendarRepository.InsertAsync(calendar);

		CalendarParticipant owner = new(Guid.NewGuid(),
			calendar.Id,
			data.CreatorId,
			DateTime.UtcNow,
			CalendarRole.Creator);
		await calendarParticipantRepository.InsertAsync(owner);

		unitOfWork.CommitTransaction();
		return calendar;
	}

	public async Task<Calendar> EditCalendarAsync(EditCalendarData data)
	{
		data.AssertNotNull();

		CalendarWithParticipants calendarInfo = await GetCalendarInfoAsync(data.CalendarId);
		CheckRequestInitiatorIsAdminOrCreator(data.EditorId, calendarInfo.Participants);
		
		string name = calendarInfo.Calendar.Name;
		string description = calendarInfo.Calendar.Description;
		
		if (!string.IsNullOrWhiteSpace(data.Name))
		{
			await CheckCalendarExistenceByName(data.Name);
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
		CheckRolesAreAdminOrParticipant(data.Users);
		CalendarWithParticipants calendarInfo = await GetCalendarInfoAsync(data.CalendarId);
		CheckRequestInitiatorIsAdminOrCreator(data.AdderId, calendarInfo.Participants);
		var userIds = data.Users.Select(x => x.UserId).ToHashSet();
		CheckUsersAreNotInCalendar(userIds, calendarInfo.Participants);
		await CheckUsersExist(userIds);

		//TODO: Здесь задается Guid.NewGuid() для удаленных участников, хотя они восстанавливаются со старым Id.
		var participantsToAdd = data.Users.Select(x =>
				new CalendarParticipant(Guid.NewGuid(),
					calendarInfo.Calendar.Id,
					x.UserId,
					DateTime.UtcNow,
					x.Role))
		   .ToHashSet();

		await calendarParticipantRepository.InsertAllAsync(participantsToAdd);

		var newParticipants = await calendarParticipantRepository.GetByCalendarIdAsync(calendarInfo.Calendar.Id);
		return new CalendarWithParticipants(calendarInfo.Calendar, newParticipants);
	}

	public async Task<CalendarWithParticipants> EditParticipantsAsync(EditCalendarParticipantsData data)
	{
		data.AssertNotNull();
		CheckRolesAreAdminOrParticipant(data.Participants);
		CalendarWithParticipants calendarInfo = await GetCalendarInfoAsync(data.CalendarId);
		CheckRequestInitiatorIsAdminOrCreator(data.UserId, calendarInfo.Participants);

		await ValidateAndUpdateParticipantsAsync(data, calendarInfo);

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

	public async Task<ICollection<CalendarParticipant>> GetParticipantsByFilterAsync(GetCalendarParticipantsByFilter data)
	{
		data.AssertNotNull();

		//TODO: Проверять, что список запрашивает участник календаря

		return await calendarParticipantRepository.GetByFilter(data.CalendarId, data.Filter, 50);
	}

	public async Task<ICollection<CalendarName>> GetCalendarNamesAsync(ISet<Guid> calendarIds)
	{
		var calendars = await calendarRepository.GetByIdsAsync(calendarIds);
		if (calendarIds.Count != calendars.Count)
		{
			throw new BusinessLogicException("Не все календари удалось найти.");
		}

		return calendars.Select(x => new CalendarName(x.Id, x.Name)).ToList();
	}

	private async Task ValidateAndUpdateParticipantsAsync(EditCalendarParticipantsData data, CalendarWithParticipants calendarInfo)
	{
		var toChange = new HashSet<CalendarParticipant>();
		var toDelete = new HashSet<Guid>();
		foreach (EditCalendarParticipantData changeParticipantRoleData in data.Participants)
		{
			CalendarParticipant? participant = calendarInfo.Participants
			   .FirstOrDefault(x => changeParticipantRoleData.ParticipantId == x.Id);
			if (participant is null)
			{
				throw new BusinessLogicException($"Участника с id {changeParticipantRoleData.ParticipantId} не найдено");
			}

			if (participant.UserId == data.UserId)
			{
				throw new BusinessLogicException("Нельзя изменить роль самого себя");
			}

			if (participant.Role.IsCreator())
			{
				throw new BusinessLogicException("Нельзя изменить роль создателя календаря");
			}

			if (!changeParticipantRoleData.Delete)
			{
				participant.Role = changeParticipantRoleData.Role!.Value;
				toChange.Add(participant);
			}
			else
			{
				toDelete.Add(participant.Id);
			}
		}

		unitOfWork.BeginTransaction();
		await calendarParticipantRepository.UpdateAllAsync(toChange);
		await calendarParticipantRepository.DeleteByIdsAsync(toDelete);
		unitOfWork.CommitTransaction();
	}

	private static void CheckNotRemovingCreator(IEnumerable<Guid> participantIds, IEnumerable<CalendarParticipant> allParticipants)
	{
		//Проверка, что нельзя удалить создателя календаря
		CalendarParticipant creator = allParticipants.Single(x => x.Role.IsCreator());
		if (participantIds.Contains(creator.Id))
		{
			throw new BusinessLogicException("Нельзя удалять создателя календаря");
		}
	}

	private static void CheckUsersAreInCalendar(IEnumerable<Guid> participantIds, IEnumerable<CalendarParticipant> allParticipants)
	{
		//Проверка, что удаляемые участники принадлежат этому календарю
		if (participantIds.Except(allParticipants.Select(x => x.Id)).Any())
		{
			//TODO: Выводить конкретный список пользователей
			throw new BusinessLogicException("Не все пользователи принадлежат этому календарю");
		}
	}

	private static void CheckRequestInitiatorIsAdminOrCreator(Guid userId, IEnumerable<CalendarParticipant> participants)
	{
		//Проверка, что редактирует администратор или создатель календаря
		CalendarParticipant? participant = participants.FirstOrDefault(x => x.UserId == userId);

		if (participant is null)
		{
			throw new BusinessLogicException("Пользователь не является участником календаря");
		}

		if (!participant.IsAdminOrCreator())
		{
			throw new BusinessLogicException("Пользователь не является создателем или администратором календаря");
		}
	}

	private static void CheckUsersAreNotInCalendar(IEnumerable<Guid> userIds, IEnumerable<CalendarParticipant> allParticipants)
	{
		//Проверка, что какие-то пользователи уже могут быть в календаре
		if (userIds.Intersect(allParticipants.Select(x => x.UserId)).Any())
		{
			//TODO: Выводить конкретный список пользователей
			throw new BusinessLogicException("Некоторые пользователи уже добавлены в календарь");
		}
	}

	private static void CheckRolesAreAdminOrParticipant(IEnumerable<AddCalendarParticipantData> users)
	{
		if (!users.All(x => x.Role.IsAdminOrParticipant()))
		{
			//TODO: Выводить конкретный список некорректных ролей
			throw new BusinessLogicException("Обнаружена некорректная роль");
		}
	}

	private static void CheckRolesAreAdminOrParticipant(IEnumerable<EditCalendarParticipantData> participants)
	{
		if (!participants.All(x => x.Role?.IsAdminOrParticipant() ?? x.Delete))
		{
			//TODO: Выводить конкретный список некорректных ролей
			throw new BusinessLogicException("Обнаружена некорректная роль");
		}
	}

	private async Task CheckUsersExist(ISet<Guid> userIds)
	{
		//Проверка, что добавляемые пользователи вообще существуют
		var users = await userRepository.GetByIdsAsync(userIds);
		if (users.Count != userIds.Count)
		{
			//TODO: Выводить конкретный список пользователей
			throw new BusinessLogicException("Найден не полный список участников");
		}
	}

	private async Task CheckCalendarExistenceByName(string name)
	{
		// Проверка на существование с таким же именем.
		Calendar? existed = await calendarRepository.GetByNameAsync(name);
		if (existed is not null)
		{
			throw new BusinessLogicException("Календарь с таким именем уже существует");
		}
	}
}