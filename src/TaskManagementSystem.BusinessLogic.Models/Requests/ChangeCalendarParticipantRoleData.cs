using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeCalendarParticipantRoleData
{
	public ChangeCalendarParticipantRoleData(Guid participantId, CalendarRole? role, bool delete)
	{
		ParticipantId = participantId;
		Role = role;
		Delete = delete;
	}

	public Guid ParticipantId { get; }

	public CalendarRole? Role { get; }
	
	public bool Delete { get; }
}