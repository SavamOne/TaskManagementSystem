using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeEventParticipantData
{
	public ChangeEventParticipantData(Guid eventParticipantId, CalendarEventParticipantRole? role, bool delete)
	{
		EventParticipantId = eventParticipantId;
		Role = role;
		Delete = delete;
	}

	public Guid EventParticipantId { get; }
	
	public CalendarEventParticipantRole? Role { get; }
	
	public bool Delete { get; }
}