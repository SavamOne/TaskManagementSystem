using System.ComponentModel.DataAnnotations.Schema;
using TaskManagementSystem.BusinessLogic.Models.Models;

namespace TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;

[Table("event_participant")]
public class DalEventParticipant
{
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    
    [Column("event_id")]
    public Guid EventId { get; set; }
    
    [Column("calendar_participant_id")]
    public Guid CalendarParticipantId { get; set; }
    
    [Column("role")]
    public int Role { get; set; }

    [ForeignKey("CalendarParticipantId")]
    public DalCalendarParticipant? CalendarParticipant { get; set; }
}