using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Helpers;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.Components.Modals;

public partial class CalendarInfoModal
{
	private readonly IEnumerable<CalendarParticipantRole> roles =
		Enum.GetValues<CalendarParticipantRole>()
		   .Where(x => x != CalendarParticipantRole.Creator)
		   .ToList();

	private CalendarViewModel calendar = new();
	private CalendarViewModel calendarForEdit = new();

	private bool changed;

	private string filter = string.Empty;
	private Dictionary<Guid, CalendarParticipantViewModel> participants = new();
	private ICollection<UserInfoWithParticipantRoleViewModel> possibleParticipants = Array.Empty<UserInfoWithParticipantRoleViewModel>();

	[Parameter]
	public Guid CalendarId { get; set; }
	
	[Parameter]
	public Action<string>? CalendarNameChanged { get; set; }

	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }

	private EditFormModal<CalendarViewModel> Modal { get; set; } = new();

	private bool Changed
	{
		get => changed;
		set
		{
			changed = value;
			StateHasChanged();
		}
	}

	private string ChangedState => Changed ? string.Empty : "disabled";

	public void Show()
	{
		Modal.Open();
	}
	
	protected override async Task OnInitializedAsync()
	{
		var result = await ServerProxy!.GetCalendarInfo(new GetCalendarInfoRequest(CalendarId));

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		calendar = new CalendarViewModel(result.Value!.Calendar);
		calendarForEdit = new CalendarViewModel(result.Value!.Calendar);

		participants = result.Value.Participants
		   .Select(x => new CalendarParticipantViewModel(x))
		   .ToDictionary(x => x.UserId);
	}

	private async Task GetUsersByFilter()
	{
		possibleParticipants.ClearIfPossible();

		var result = await ServerProxy!.GetUsersByFilterAsync(new GetUserInfosByFilterRequest(filter));

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		if (!result.Value!.Any())
		{
			ToastService!.AddSystemToast("Календарь", "Не найден список участников по запросу");
			return;
		}

		possibleParticipants = result.Value!
		   .Where(x => !participants.ContainsKey(x.Id))
		   .Select(x => new UserInfoWithParticipantRoleViewModel(x))
		   .OrderByDescending(x => x.Role)
		   .ThenBy(x => x.Name)
		   .ToList();
	}

	private async Task SaveChanges()
	{
		if (!Changed)
		{
			return;
		}

		await TryEditCalendarInfo();
		await TryAddToParticipants();
		await TryEditParticipants();

		Changed = false;
	}

	private async Task<bool> TryEditParticipants()
	{
		var participantsWithChangedRole = participants.Values
		   .Where(x => x.RoleChanged)
		   .ToList();

		if (!participantsWithChangedRole.Any())
		{
			return false;
		}

		CalendarWithParticipantUsers? newParticipantsList = await TryEdit(participantsWithChangedRole);

		if (newParticipantsList is null)
		{
			return false;
		}

		participants = newParticipantsList.Participants
		   .Select(x => new CalendarParticipantViewModel(x))
		   .ToDictionary(x => x.UserId);

		return true;
	}
	
	private async Task<CalendarWithParticipantUsers?> TryEdit(IEnumerable<CalendarParticipantViewModel> participantsWithChangedRole)
	{
		var toEditRoleParticipants = participantsWithChangedRole
		   .Select(x => x.GetChangeRoleRequest())
		   .ToList();

		if (!toEditRoleParticipants.Any())
		{
			return null;
		}

		var changeRoleResult = await ServerProxy!.EditCalendarParticipants(new EditCalendarParticipantsRequest(CalendarId, toEditRoleParticipants));

		if (!changeRoleResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(changeRoleResult.ErrorDescription!);
			return null;
		}

		ToastService!.AddSystemToast("Календарь", "Роли успешно изменены.");
		return changeRoleResult.Value;
	}

	private async Task TryAddToParticipants()
	{
		var toAddRequests = possibleParticipants
		   .Where(x => x.Role != CalendarParticipantRole.NotSet)
		   .Select(x => x.GetAddParticipantRequest())
		   .ToList();

		if (!toAddRequests.Any())
		{
			return;
		}

		var addResult = await ServerProxy!.AddCalendarParticipants(new AddCalendarParticipantsRequest(CalendarId, toAddRequests));

		if (!addResult.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(addResult.ErrorDescription!);
			return;
		}

		participants = addResult.Value!.Participants
		   .Select(x => new CalendarParticipantViewModel(x))
		   .ToDictionary(x => x.UserId);

		filter = string.Empty;
		possibleParticipants.Clear();

		ToastService!.AddSystemToast("Календарь", "Новые участники календаря добавлены");
	}

	private async Task TryEditCalendarInfo()
	{
		if (calendar.Equals(calendarForEdit))
		{
			return;
		}

		var result = await ServerProxy!.EditCalendar(calendarForEdit.GetEditRequest());

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		calendar = new CalendarViewModel(result.Value!);

		ToastService!.AddSystemToast("Календарь", "Информация о календаре обновлена");
		
		CalendarNameChanged?.Invoke(calendar.Name!);
	}

	private void ChangeCalendarName(string newName)
	{
		calendarForEdit.Name = newName;
		Changed |= !calendarForEdit.Equals(calendar);
	}

	private void ChangeCalendarDescription(string newDescription)
	{
		calendarForEdit.Description = newDescription;
		Changed |= !calendarForEdit.Equals(calendar);
	}
}