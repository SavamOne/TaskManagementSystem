using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.BusinessLogic.Dal.DataAccessModels;

[Table("user")]
public class DalUser
{
	[Column("id")]
	[DatabaseGenerated(DatabaseGeneratedOption.None)]
	public Guid Id { get; set; }

	[Column("name")]
	public string Name { get; set; }

	[Column("email")]
	public string Email { get; set; }

	[Column("register_date")]
	public DateTime RegisterDate { get; set; }

	[Column("password_hash")]
	public byte[] PasswordHash { get; set; }

	[Column("is_deleted")]
	public bool IsDeleted { get; set; }
}