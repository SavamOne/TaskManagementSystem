using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Server.Dal.DalModels;

[Table("user_token")]
public class DalUserToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("user_id")]
    public Guid UserId { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("refresh_token")]
    public string RefreshToken { get; set; }
}