using TaskManagementSystem.Client.Extensions;
using TaskManagementSystem.Client.Helpers.Implementations;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class EventInfoViewModel
{
	private static readonly Dictionary<Guid, string> CalendarNames = new();

	public EventInfoViewModel(EventInfo data)
	{
		this.Data = data;
	}

	public string Name => Data.Name;

	public DateTime StartTime => Data.StartTime.LocalDateTime;

	public DateTime EndTime => Data.EndTime.LocalDateTime;

	public CalendarEventType EventType => Data.EventType;

	public string Icon => EventType.GetIcon();

	public string Color => CalendarNames.TryGetValue(Data.CalendarId, out string? name) ? StringHelper.GetHexColorForText(name) : string.Empty;

	public string Opacity => EndTime >= DateTime.Now ? "1.0" : "0.85";

	public EventInfo Data { get; }

	public static void AddCalendarName(Guid calendarId, string name)
	{
		CalendarNames[calendarId] = name;
	}

	public static ISet<Guid> GetUnknownCalendarNames(IEnumerable<EventInfo> eventInfos)
	{
		return eventInfos.Select(x => x.CalendarId).Distinct().Except(CalendarNames.Keys).ToHashSet();
	}
}