using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Resources;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Components;

public partial class LocalizationSettingsComponent
{
	private bool cultureChanged;
	private IEnumerable<CultureViewModel>? cultureItems;

	private bool firstDayChanged;
	private IEnumerable<DayOfWeekViewModel>? firstDayItems;
	private CultureViewModel? selectedCulture;
	private DayOfWeekViewModel? selectedFirstDay;

	[Inject]
	public ILocalizationService? LocalizationService { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }

	[Inject]
	public NavigationManager? NavigationManager { get; set; }

	private bool IsLoaded => cultureItems is not null && firstDayItems is not null;

	protected override async Task OnInitializedAsync()
	{
		selectedCulture = new CultureViewModel(await LocalizationService!.GetApplicationCultureAsync());
		cultureItems = LocalizationService!.GetAvailableCultures().Select(x => new CultureViewModel(x)).ToList();

		selectedFirstDay = new DayOfWeekViewModel(await LocalizationService!.GetApplicationFirstDayOfWeekAsync(), false, selectedCulture.Value);
		firstDayItems = Enum.GetValues<DayOfWeek>().Select(x => new DayOfWeekViewModel(x, false, selectedCulture.Value));
	}

	private void ChangeCulture(CultureViewModel viewModel)
	{
		selectedCulture = viewModel;
		cultureChanged = true;
		StateHasChanged();
	}

	private void ChangeFirstDay(DayOfWeekViewModel viewModel)
	{
		selectedFirstDay = viewModel;
		firstDayChanged = true;
		StateHasChanged();
	}

	private async Task Save()
	{
		if (firstDayChanged)
		{
			await LocalizationService!.SetApplicationFirstDayOfWeekAsync(selectedFirstDay!.Value);
			firstDayChanged = false;
		}

		if (cultureChanged)
		{
			await LocalizationService!.SetApplicationCultureAsync(selectedCulture!.Value);
			StateHasChanged();
		}

		ToastService!.AddSystemToast(LocalizedResources.LocalizationSettingsComponent_SettingsChange, LocalizedResources.LocalizationSettingsComponent_SettingsSuccessfullyChanged);

		if (cultureChanged)
		{
			await Task.Delay(1000);
			NavigationManager!.NavigateTo(NavigationManager.Uri, true);
			cultureChanged = false;
		}
	}
}