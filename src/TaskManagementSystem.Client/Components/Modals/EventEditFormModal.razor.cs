using System.Globalization;
using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Helpers.Implementations;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Components.Modals;

public partial class EventEditFormModal
{
	private readonly IEnumerable<CalendarEventType> eventTypes = Enum.GetValues<CalendarEventType>();

	private readonly IEnumerable<EventParticipantRole> roles = Enum.GetValues<EventParticipantRole>()
	   .Where(x => x != EventParticipantRole.Creator)
	   .ToList();

	private readonly IEnumerable<EventRepeatType> repeatTypes = Enum.GetValues<EventRepeatType>();

	private IEnumerable<DayOfWeekViewModel>? dayOfWeeks;

	private string filter = string.Empty;

	private bool isEditMode, participantsChanged;

	private ICollection<EventParticipantViewModel> participants = Array.Empty<EventParticipantViewModel>();
	private ICollection<UserInfoWithEventRoleViewModel> possibleParticipants = Array.Empty<UserInfoWithEventRoleViewModel>();

	private bool isRepeated, notifyRepeatChanged;
	private string repeatedStartDateStr = string.Empty;
	private string repeatedEndDateStr = string.Empty;

	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }
	
	[Inject]
	public ILocalizationService? LocalizationService { get; set; }

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

	protected override async Task OnInitializedAsync()
	{
		CultureInfo culture = await LocalizationService!.GetApplicationCultureAsync();
		dayOfWeeks = DayOfWeekHelper.GetDayOfWeeksOrderedByFirstDay(culture, false);
	}
	
	public async Task EditAsync(EventInfo eventInfo)
	{
		isEditMode = true;
		Modal.Title = "Редактирование события";
		participantsChanged = false;

		isRepeated = eventInfo.IsRepeated;
		notifyRepeatChanged = false;
		repeatedStartDateStr = eventInfo.StartTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");
		repeatedEndDateStr = eventInfo.EndTime.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

		EventWithParticipants? result = await GetEventInfoAsync(eventInfo.Id);

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

		isRepeated = false;
		notifyRepeatChanged = false;
		repeatedStartDateStr = string.Empty;
		repeatedEndDateStr = string.Empty;

		Event = new EventViewModel();
		participants.ClearIfPossible();
		possibleParticipants.ClearIfPossible();
		filter = string.Empty;

		Modal.Open();
	}

	private async Task SubmitAsync()
	{
		Guid eventId = default;
		
		if (CanUserChangeEvent && Event.Changed)
		{
			EventInfo? eventInfo = await CreateOrEditEventAsync();

			if (eventInfo is not null)
			{
				eventId = eventInfo.Id;
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
		else if (eventId != default)
		{
			eventWithParticipants = await GetEventInfoAsync(eventId);
		}

		if (eventWithParticipants is not null)
		{
			notifyRepeatChanged = isRepeated;
			isRepeated = false;
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
		Event = new EventViewModel(eventWithParticipants.EventInfo, eventWithParticipants.RecurrentSettings, eventWithParticipants.CanUserEditEvent, eventWithParticipants.CanUserEditParticipants, eventWithParticipants.CanUserDeleteEvent);
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