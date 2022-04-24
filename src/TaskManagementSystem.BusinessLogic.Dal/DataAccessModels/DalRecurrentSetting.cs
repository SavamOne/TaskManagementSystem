using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;

[Table("recurrent_event_setting")]
public class DalRecurrentSetting
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("event_id")]
	public Guid EventId { get; set; }

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("key")]
	public string Key { get; set; }

	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	[Column("value")]
	public long Value { get; set; }
}