using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class AddRecurrentSettingsData
{
	public AddRecurrentSettingsData(RepeatType repeatType, ISet<DayOfWeek>? dayOfWeeks, DateTimeOffset? until,
		uint? count)
	{
		RepeatType = repeatType;
		DayOfWeeks = dayOfWeeks;
		Until = until;
		Count = count;
	}

	public RepeatType RepeatType { get; }
	
	public ISet<DayOfWeek>? DayOfWeeks { get; }
	
	public DateTimeOffset? Until { get; }
	
	public uint? Count { get; }
}