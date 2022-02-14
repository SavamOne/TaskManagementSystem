using System.Globalization;

namespace TaskManagementSystem.Client.ViewModels;

public class FirstDayOfWeekViewModel
{
    private readonly CultureInfo cultureInfo;

    public FirstDayOfWeekViewModel(DayOfWeek firstDay, CultureInfo cultureInfo)
    {
        Value = firstDay;
        this.cultureInfo = cultureInfo;
    }

    public DayOfWeek Value { get; }

    public override string ToString()
    {
        string name = cultureInfo.DateTimeFormat.GetDayName(Value);
        return cultureInfo.TextInfo.ToTitleCase(name);
    }
}