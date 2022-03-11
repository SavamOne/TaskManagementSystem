using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;

[Table("calendar")]
public class DalCalendar
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("description")]
    public string Description { get; set; }

    [Column("creation_date")]
    public DateTime CreationDate { get; set; }

    [Column("is_deleted")]
    public bool IsDeleted { get; set; }
}