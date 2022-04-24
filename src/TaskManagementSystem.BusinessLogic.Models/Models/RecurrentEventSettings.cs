namespace TaskManagementSystem.BusinessLogic.Models.Models;

public class RecurrentEventSettings
{
	public RecurrentEventSettings(Guid eventId,
		RepeatType repeatType,
		DateTime? untilUtc,
		uint? repeatCount,
		ISet<DayOfWeek>? dayOfWeeks)
	{
		RepeatType = repeatType;
		UntilUtc = untilUtc;
		RepeatCount = repeatCount;
		DayOfWeeks = dayOfWeeks;
		EventId = eventId;
	}

	public Guid EventId { get; }

	public RepeatType RepeatType { get; set; }

	public DateTime? UntilUtc { get; set; }

	public uint? RepeatCount { get; set; }

	public ISet<DayOfWeek>? DayOfWeeks { get; set; }
}