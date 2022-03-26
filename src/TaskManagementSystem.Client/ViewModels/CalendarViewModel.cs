using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class CalendarViewModel
{
	public CalendarViewModel() {}

	public CalendarViewModel(CalendarInfo calendarInfo)
	{
		Id = calendarInfo.Id;
		Name = calendarInfo.Name;
		Description = calendarInfo.Description;
	}

	public Guid? Id { get; }

	public string? Name { get; set; }

	public string? Description { get; set; }

	public bool Equals(CalendarViewModel? other)
	{
		return string.Equals(other?.Name, Name) && string.Equals(other?.Description, Description);
	}

	public CreateCalendarRequest GetRequest()
	{
		return new CreateCalendarRequest(Name!, Description!);
	}

	public EditCalendarRequest GetEditRequest()
	{
		return new EditCalendarRequest(Id!.Value, Name!, Description!);
	}
}