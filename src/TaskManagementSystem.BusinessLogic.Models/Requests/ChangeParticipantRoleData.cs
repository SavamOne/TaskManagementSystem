using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Models.Requests;

public class ChangeParticipantRoleData
{
	public ChangeParticipantRoleData(Guid participantId, CalendarRole role)
	{
		ParticipantId = participantId;
		Role = role;
	}

	public Guid ParticipantId { get; }

	public CalendarRole Role { get; }
}