using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;

[Table("event")]
public class DalEvent
{
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("id")]
	public Guid Id { get; set; }

	[Column("calendar_id")]
	public Guid CalendarId { get; set; }

	[Column("event_type")]
	public int EventType { get; set; }

	[Column("name")]
	public string Name { get; set; }

	[Column("description")]
	public string? Descrption { get; set; }

	[Column("place")]
	public string? Place { get; set; }

	[Column("start_time")]
	public DateTime StartTime { get; set; }

	[Column("end_time")]
	public DateTime EndTime { get; set; }

	[Column("is_private")]
	public bool IsPrivate { get; set; }

	[Column("creation_time")]
	public DateTime CreationTime { get; set; }
	
	[Column("is_repeated")]
	public bool IsRepeated { get; set; }
}