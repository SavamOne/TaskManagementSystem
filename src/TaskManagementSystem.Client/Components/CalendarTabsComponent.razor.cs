using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Components;

public partial class CalendarTabsComponent
{
	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }

	public IEnumerable<CalendarInfo> Calendars { get; set; } = Enumerable.Empty<CalendarInfo>();

	protected override async Task OnInitializedAsync()
	{
		await RefreshCalendarsStateAsync();
	}

	public async Task RefreshCalendarsStateAsync()
	{
		var result = await ServerProxy!.GetUserCalendars();

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		Calendars = result.Value!;
		StateHasChanged();
	}
}