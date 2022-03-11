using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Helpers;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Components;

public partial class CalendarInfoComponent
{
    private const string CalendarDescriptionId = "CalendarDescriptionId";
    private const string CalendarNameId = "CalendarNameId";

    private readonly IEnumerable<CalendarParticipantRole> roles =
        Enum.GetValues<CalendarParticipantRole>()
            .Where(x => x != CalendarParticipantRole.Creator)
            .ToList();

    private CalendarViewModel calendar = new();
    private CalendarViewModel calendarForEdit = new();

    private bool changed, isNameEditing, isDescriptionEditing;

    private string filter = string.Empty;
    private Dictionary<Guid, CalendarParticipantViewModel> participants = new();

    private ICollection<UserInfoWithRoleViewModel> possibleParticipants = new List<UserInfoWithRoleViewModel>();

    [Parameter]
    public Guid CalendarId { get; set; }

    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    [Inject]
    public IJSInteropWrapper? JsInteropWrapper { get; set; }

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
        possibleParticipants.Clear();

        var result = await ServerProxy!.GetUsersByFilterAsync(new GetUserInfosByFilterRequest(filter ?? string.Empty));

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
            .Select(x => new UserInfoWithRoleViewModel(x))
            .OrderBy(x => x.Role)
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
        await TryChangeRoleOrDelete();

        Changed = false;
    }

    private async Task<bool> TryChangeRoleOrDelete()
    {
        var participantsWithChangedRole = participants.Values
            .Where(x => x.RoleChanged)
            .ToList();

        if (!participantsWithChangedRole.Any())
        {
            return false;
        }

        CalendarWithParticipantUsers? newParticipantsList = await TryChangeRole(participantsWithChangedRole);
        newParticipantsList = await TryDelete(participantsWithChangedRole) ?? newParticipantsList;


        if (newParticipantsList is null)
        {
            return false;
        }

        participants = newParticipantsList.Participants
            .Select(x => new CalendarParticipantViewModel(x))
            .ToDictionary(x => x.UserId);

        return true;
    }

    private async Task<CalendarWithParticipantUsers?> TryDelete(IEnumerable<CalendarParticipantViewModel> participantsWithChangedRole)
    {
        var toDeleteIds = participantsWithChangedRole
            .Where(x => x.Role == CalendarParticipantRole.NotSet)
            .Select(x => x.ParticipantId)
            .ToList();

        if (!toDeleteIds.Any())
        {
            return null;
        }

        var deleteResult = await ServerProxy!.DeleteCalendarParticipants(new DeleteParticipantsRequest(CalendarId, toDeleteIds));

        if (!deleteResult.IsSuccess)
        {
            ToastService!.AddSystemErrorToast(deleteResult.ErrorDescription!);
            return null;
        }

        ToastService!.AddSystemToast("Календарь", "Пользователи успешно удалены");
        return deleteResult.Value;
    }

    private async Task<CalendarWithParticipantUsers?> TryChangeRole(IEnumerable<CalendarParticipantViewModel> participantsWithChangedRole)
    {
        var toChangeRoleRequests = participantsWithChangedRole
            .Where(x => x.Role != CalendarParticipantRole.NotSet)
            .Select(x => x.GetChangeRoleRequest())
            .ToList();

        if (!toChangeRoleRequests.Any())
        {
            return null;
        }

        var changeRoleResult = await ServerProxy!.ChangeParticipantsRole(new ChangeCalendarParticipantsRoleRequest(CalendarId, toChangeRoleRequests));

        if (!changeRoleResult.IsSuccess)
        {
            ToastService!.AddSystemErrorToast(changeRoleResult.ErrorDescription!);
            return null;
        }

        ToastService!.AddSystemToast("Календарь", "Роли успешно изменены");
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
    }

    public string RoleStr(CalendarParticipantRole role)
    {
        return role switch
        {
            CalendarParticipantRole.Admin => "Администратор",
            CalendarParticipantRole.Creator => "Создатель календаря",
            CalendarParticipantRole.Participant => "Участник",
            CalendarParticipantRole.NotSet => "Не участник",
            _ => throw new ArgumentOutOfRangeException(nameof(RoleStr))

        };
    }

    private async Task ChangeCalendarName()
    {
        if (!isNameEditing)
        {
            isNameEditing = true;
            return;
        }

        calendarForEdit.Name = await JsInteropWrapper!.GetInnerTextByIdAsync(CalendarNameId);

        Changed |= !calendarForEdit.Equals(calendar);
        isNameEditing = false;
    }

    private async Task ChangeCalendarDescription()
    {
        if (!isDescriptionEditing)
        {
            isDescriptionEditing = true;
            return;
        }

        calendarForEdit.Description = await JsInteropWrapper!.GetInnerTextByIdAsync(CalendarDescriptionId);

        Changed |= !calendarForEdit.Equals(calendar);
        isDescriptionEditing = false;
    }
}