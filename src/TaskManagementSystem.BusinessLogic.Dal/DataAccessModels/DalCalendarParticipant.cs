using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;

[Table("calendar_participant")]
public class DalCalendarParticipant
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("calendar_id")]
    public Guid CalendarId { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }

    [Column("join_date")]
    public DateTime JoinDate { get; set; }

    [Column("role")]
    public int Role { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}