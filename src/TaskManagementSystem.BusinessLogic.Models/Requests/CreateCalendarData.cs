using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class CreateCalendarData
{
	public CreateCalendarData(Guid creatorId, string name, string description)
	{
		CreatorId = creatorId;
		Name = name.AssertNotNullOrWhiteSpace();
		Description = description.AssertNotNullOrWhiteSpace();
	}

	public Guid CreatorId { get; }

	public string Name { get; }

	public string Description { get; }
}