using TaskManagementSystem.Shared.Models;
using TaskManagementSystem.Shared.Models.Requests;

namespace TaskManagementSystem.Client.ViewModels;

public class EventParticipantViewModel
{
	private readonly EventParticipantUser eventParticipantUser;

	private EventParticipantRole role;

	public EventParticipantViewModel(EventParticipantUser eventParticipantUser)
	{
		this.eventParticipantUser = eventParticipantUser;

		role = eventParticipantUser.Role;
		UserId = eventParticipantUser.UserId;
		EventParticipantId = eventParticipantUser.EventParticipantId;
		Name = eventParticipantUser.UserName;
		Email = eventParticipantUser.UserEmail;
	}

	public bool RoleChanged { get; private set; }

	public Guid UserId { get; }

	public Guid EventParticipantId { get; }

	public string Name { get; }

	public string Email { get; }

	public EventParticipantRole Role
	{
		get => role;
		set
		{
			RoleChanged = true;
			role = value;
		}
	}

	public ChangeEventParticipantRequest GetChangeRequest()
	{
		if (!RoleChanged)
		{
			throw new Exception();
		}

		return new ChangeEventParticipantRequest(EventParticipantId,
			Role != EventParticipantRole.NotSet ? Role : null,
			Role == EventParticipantRole.NotSet);
	}
}