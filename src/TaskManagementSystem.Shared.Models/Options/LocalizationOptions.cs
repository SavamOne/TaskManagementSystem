using System.Globalization;

namespace TaskManagementSystem.Shared.Models.Options;

public static class LocalizationOptions
{
	private static readonly Dictionary<string, CultureInfo> SupportedCultures = new(StringComparer.InvariantCultureIgnoreCase)
	{
		// {
		// 	"en", CultureInfo.GetCultureInfo("en")
		// },
		{
			"ru", CultureInfo.GetCultureInfo("ru")
		}
	};

	public static CultureInfo DefaultCultureInfo { get; } = SupportedCultures.First().Value;

	public static List<CultureInfo> AvailableCultureInfos { get; } = SupportedCultures.Values.ToList();

	public static CultureInfo GetCultureByNameOrDefault(string name)
	{
		if (SupportedCultures.TryGetValue(name, out CultureInfo? value))
		{
			return value!;
		}

		return DefaultCultureInfo;
	}
}