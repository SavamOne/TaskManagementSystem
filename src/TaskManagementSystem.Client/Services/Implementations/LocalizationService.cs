using System.Globalization;
using TaskManagementSystem.Client.Helpers;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models.Options;

namespace TaskManagementSystem.Client.Services.Implementations;

public class LocalizationService : ILocalizationService
{
    private const string CultureKey = "culture";
    private const string FirstDayOfWeekKey = "first_day_of_week";

    private readonly IJSInteropWrapper wrapper;

    private bool initialized;

    private CultureInfo? usedCulture;
    private DayOfWeek? usedFirstDayOfWeek;

    public LocalizationService(IJSInteropWrapper wrapper)
    {
        this.wrapper = wrapper.AssertNotNull();
    }

    public async Task InitializeAsync()
    {
        CultureInfo defaultCulture = Clone(CultureInfo.CurrentCulture);

        usedFirstDayOfWeek = await GetFirstDayOfWeekFromStorageAsync() ?? defaultCulture.DateTimeFormat.FirstDayOfWeek;
        usedCulture = await GetCultureInfoFromStorageAsync() ?? defaultCulture;
        usedCulture.DateTimeFormat.FirstDayOfWeek = usedFirstDayOfWeek!.Value;

        CultureInfo.DefaultThreadCurrentCulture = usedCulture;
        CultureInfo.DefaultThreadCurrentUICulture = usedCulture;

        initialized = true;
    }

    public async Task<CultureInfo> GetApplicationCultureAsync()
    {
        if (!initialized)
        {
            await InitializeAsync();
        }

        return usedCulture!;
    }

    public async Task<DayOfWeek> GetApplicationFirstDayOfWeekAsync()
    {
        if (!initialized)
        {
            await InitializeAsync();
        }

        return usedFirstDayOfWeek!.Value;
    }

    public async Task<DateTimeFormatInfo> GetApplicationDateTimeFormatAsync()
    {
        if (!initialized)
        {
            await InitializeAsync();
        }

        return usedCulture!.DateTimeFormat;
    }

    public async Task<string> GeApplicationCultureNameAsync()
    {
        if (!initialized)
        {
            await InitializeAsync();
        }

        return usedCulture!.TwoLetterISOLanguageName;
    }

    public async Task SetApplicationCultureAsync(CultureInfo culture)
    {
        if (!initialized)
        {
            await InitializeAsync();
        }

        usedCulture = Clone(culture);
        usedCulture.DateTimeFormat.FirstDayOfWeek = usedFirstDayOfWeek!.Value;

        CultureInfo.DefaultThreadCurrentCulture = usedCulture;
        CultureInfo.DefaultThreadCurrentUICulture = usedCulture;

        await SetCultureInfoToStorageAsync(usedCulture);
    }

    public async Task SetApplicationFirstDayOfWeekAsync(DayOfWeek dayOfWeek)
    {
        if (!initialized)
        {
            await InitializeAsync();
        }

        usedFirstDayOfWeek = dayOfWeek;
        usedCulture!.DateTimeFormat.FirstDayOfWeek = usedFirstDayOfWeek.Value;

        await SetFirstDayOfWeekToStorageAsync(usedFirstDayOfWeek.Value);
    }

    public IEnumerable<CultureInfo> GetAvailableCultures()
    {
        return LocalizationOptions.AvailableCultureInfos;
    }

    private async Task<DayOfWeek?> GetFirstDayOfWeekFromStorageAsync()
    {
        string? value = await wrapper.GetStringAsync(FirstDayOfWeekKey);

        if (Enum.TryParse(value, out DayOfWeek dayOfWeek))
        {
            return dayOfWeek;
        }

        return null;
    }

    private async Task<CultureInfo?> GetCultureInfoFromStorageAsync()
    {
        string? value = await wrapper.GetStringAsync(CultureKey);

        if (!string.IsNullOrEmpty(value))
        {
            return Clone(LocalizationOptions.GetCultureByNameOrDefault(value));
        }

        return null;
    }

    private async Task SetCultureInfoToStorageAsync(CultureInfo culture)
    {
        await wrapper.SetStringAsync(CultureKey, culture.TwoLetterISOLanguageName);
    }

    private async Task SetFirstDayOfWeekToStorageAsync(DayOfWeek firstDayOfWeek)
    {
        await wrapper.SetStringAsync(FirstDayOfWeekKey, firstDayOfWeek.ToString());
    }

    private CultureInfo Clone(CultureInfo culture)
    {
        return ( culture.Clone() as CultureInfo )!;
    }
}