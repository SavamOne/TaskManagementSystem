namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class GetEventInfoData
{
	public GetEventInfoData(Guid userId, Guid eventId)
	{
		UserId = userId;
		EventId = eventId;
	}

	public Guid UserId { get; }

	public Guid EventId { get; }
}