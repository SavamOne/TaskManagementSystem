using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Components.Modals;

public partial class EventEditFormModal
{

	private IEnumerable<CalendarEventType> eventTypes = Enum.GetValues<CalendarEventType>();

	private string filter = string.Empty;

	private bool isEditMode, participantsChanged;

	private ICollection<EventParticipantViewModel> participants = Array.Empty<EventParticipantViewModel>();
	private ICollection<UserInfoWithEventRoleViewModel> possibleParticipants = Array.Empty<UserInfoWithEventRoleViewModel>();

	private IEnumerable<EventParticipantRole> roles = Enum.GetValues<EventParticipantRole>()
	   .Where(x => x != EventParticipantRole.Creator)
	   .ToList();

	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }

	[Parameter]
	public Guid CalendarId { get; set; }

	[Parameter]
	public Func<Task> OnEventChanged { get; set; } = () => Task.CompletedTask;

	private EditFormModal<EventViewModel> Modal { get; set; } = new();

	private EventViewModel Event { get; set; } = new();

	private bool ParticipantsChanged
	{
		get => participantsChanged;
		set
		{
			participantsChanged = true;
			StateHasChanged();
		}
	}

	private bool CanUserChangeEvent => Event.CanEditEvent;

	private bool CanUserChangeParticipants => Event.CanChangeParticipants;

	private bool CanUserDeleteEvent => Event.CanDeleteEvent;

	public async Task EditAsync(Guid eventId)
	{
		isEditMode = true;
		Modal.Title = "Редактирование события";
		participantsChanged = false;

		EventWithParticipants? result = await GetEventInfoAsync(eventId);

		if (result is null)
		{
			return;
		}

		Fill(result);
		Modal.Open();
	}

	public void Create()
	{
		isEditMode = false;
		Modal.Title = "Создание события";
		participantsChanged = false;

		Event = new EventViewModel();
		participants.ClearIfPossible();
		possibleParticipants.ClearIfPossible();
		filter = string.Empty;

		Modal.Open();
	}

	private async Task SubmitAsync()
	{
		if (CanUserChangeEvent && Event.Changed)
		{
			EventInfo? eventInfo = await CreateOrEditEventAsync();

			if (eventInfo is not null)
			{
				Event = new EventViewModel(eventInfo, true, true, false);
				isEditMode = true;
			}
		}

		EventWithParticipants? eventWithParticipants = null;
		if (CanUserChangeParticipants && participantsChanged)
		{
			eventWithParticipants = await AddParticipantsAsync();
			eventWithParticipants = await ChangeParticipantsAsync() ?? eventWithParticipants;

			participantsChanged = false;
		}
		else if (Event.Id != default)
		{
			eventWithParticipants = await GetEventInfoAsync(Event.Id);
		}

		if (eventWithParticipants is not null)
		{
			Fill(eventWithParticipants);
			StateHasChanged();
			await OnEventChanged();
		}
	}

	private async Task<EventWithParticipants?> ChangeParticipantsAsync()
	{
		if (!CanUserChangeParticipants)
		{
			return null;
		}

		var changeRequests = participants
		   .Where(x => x.RoleChanged)
		   .Select(x => x.GetChangeRequest())
		   .ToList();

		if (!changeRequests.Any())
		{
			return null;
		}

		var changeResult = await ServerProxy!.ChangeEventParticipants(new ChangeEventParticipantsRequest(Event.Id, changeRequests));
		if (!changeResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(changeResult.ErrorDescription!);
			return null;
		}

		ToastService!.AddSystemToast(Modal.Title!, "Успешно изменены участники события");

		return changeResult.Value;
	}

	private async Task<EventWithParticipants?> AddParticipantsAsync()
	{
		if (!CanUserChangeParticipants)
		{
			return null;
		}

		var addRequests = possibleParticipants
		   .Where(x => x.Role != EventParticipantRole.NotSet)
		   .Select(x => x.GetAddParticipantRequest())
		   .ToList();

		if (!addRequests.Any())
		{
			return null;
		}

		var addResult = await ServerProxy!.AddEventParticipants(new AddEventParticipantsRequest(Event.Id, addRequests));
		if (!addResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(addResult.ErrorDescription!);
			return null;
		}

		ToastService!.AddSystemToast(Modal.Title!, "Успешно добавлены участники события");

		return addResult.Value;
	}

	private async Task<EventWithParticipants?> GetEventInfoAsync(Guid eventId)
	{
		var getInfoResult = await ServerProxy!.GetEventInfo(new GetEventInfoRequest(eventId));
		if (!getInfoResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(getInfoResult.ErrorDescription!);
			return null;
		}

		return getInfoResult.Value;
	}

	private async Task<EventInfo?> CreateOrEditEventAsync()
	{
		if (!CanUserChangeEvent)
		{
			return null;
		}

		Result<EventInfo> result;

		if (isEditMode)
		{
			result = await ServerProxy!.EditEvent(Event.GetEditRequest());
		}
		else
		{
			result = await ServerProxy!.CreateEvent(Event.GetCreateRequest(CalendarId));
		}

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return null;
		}

		ToastService!.AddSystemToast(Modal.Title!, "Успешно завершено");

		return result.Value;
	}

	private async Task FilterParticipantsAsync()
	{
		var result = await ServerProxy!.GetCalendarParticipantsByFilter(new GetCalendarParticipantsByFilterRequest(CalendarId, filter));

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
		}

		possibleParticipants = result.Value!
		   .Where(x => participants.All(y => x.UserId != y.UserId))
		   .Select(x => new UserInfoWithEventRoleViewModel(x))
		   .ToList();
	}

	private void Fill(EventWithParticipants eventWithParticipants)
	{
		Event = new EventViewModel(eventWithParticipants.EventInfo, eventWithParticipants.CanUserEditEvent, eventWithParticipants.CanUserEditParticipants, eventWithParticipants.CanUserDeleteEvent);
		participants = eventWithParticipants.Participants
		   .OrderByDescending(x => x.Role)
		   .ThenBy(x => x.UserName)
		   .Select(x => new EventParticipantViewModel(x))
		   .ToList();

		possibleParticipants.ClearIfPossible();
		filter = string.Empty;
	}

	private async Task DeleteEventAsync()
	{
		var result = await ServerProxy!.DeleteEvent(new DeleteEventRequest(Event.Id));
		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		ToastService!.AddSystemToast(Modal.Title!, "Событие удалено успешно");
		await OnEventChanged();
		Modal.Close();
	}
}