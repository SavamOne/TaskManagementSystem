using System.Globalization;

namespace TaskManagementSystem.Client.ViewModels;

public class CultureViewModel
{
	public CultureViewModel(CultureInfo cultureInfo)
	{
		Value = cultureInfo;
	}

	public CultureInfo Value { get; }

	public override string ToString()
	{
		return Value.DisplayName;
	}
}