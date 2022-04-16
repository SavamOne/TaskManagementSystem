using TaskManagementSystem.Client.Extensions;
using TaskManagementSystem.Client.Helpers.Implementations;
using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class EventInfoViewModel
{
	private static readonly Dictionary<Guid, string> CalendarNames = new();
	
	private readonly EventInfo data;

	public EventInfoViewModel(EventInfo data)
	{
		this.data = data;
	}

	public string Name => data.Name;
	
	public DateTime StartTime => data.StartTime.LocalDateTime;

	public DateTime EndTime => data.EndTime.LocalDateTime;

	public CalendarEventType EventType => data.EventType;

	public string Icon => EventType.GetIcon();

	public string Color => CalendarNames.TryGetValue(data.CalendarId, out string? name) ? StringHelper.GetHexColorForText(name) : string.Empty;

	public string Opacity => EndTime >= DateTime.Now ? "1.0" : "0.85";

	public EventInfo Data => data;
	
	public static void AddCalendarName(IEnumerable<CalendarNameResponse> responses)
	{
		foreach (CalendarNameResponse response in responses)
		{
			CalendarNames.TryAdd(response.CalendarId, response.Name);
		}
	}
	
	public static ISet<Guid> GetUnknownCalendarNames(IEnumerable<EventInfo> eventInfos)
	{
		return eventInfos.Select(x=> x.CalendarId).Distinct().Except(CalendarNames.Keys).ToHashSet();
	}
}