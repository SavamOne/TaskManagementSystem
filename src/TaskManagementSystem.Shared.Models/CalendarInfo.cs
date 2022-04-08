using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
/// Информация о календаре.
/// </summary>
public class CalendarInfo
{
	public CalendarInfo(Guid id,
		string name,
		string description,
		DateTimeOffset creationDate)
	{
		Id = id;
		Name = name.AssertNotNullOrWhiteSpace();
		Description = description.AssertNotNullOrWhiteSpace();
		CreationDate = creationDate;
	}

	/// <summary>
	/// Id календаря.
	/// </summary>
	[Required]
	public Guid Id { get; }

	/// <summary>
	/// Имя.
	/// </summary>
	[Required]
	public string Name { get; }

	/// <summary>
	/// Описание.
	/// </summary>
	[Required]
	public string Description { get; }

	/// <summary>
	/// Дата создания.
	/// </summary>
	[Required]
	public DateTimeOffset CreationDate { get; }
}