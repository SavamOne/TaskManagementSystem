using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Server.Dal.DalModels;

[Table("notification_subscription")]
public class DalNotificationSubscription
{
	[Key]
	[Column("user_id")]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public Guid UserId { get; set; }
	
	[Key]
	[Column("url")]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public string Url { get; set; }

	[Column("p256dh")]
	public string P256dh { get; set; }

	[Column("auth")]
	public string Auth { get; set; }
}