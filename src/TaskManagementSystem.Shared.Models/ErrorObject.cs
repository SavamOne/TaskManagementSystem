using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Shared.Helpers;

namespace TaskManagementSystem.Shared.Models;

/// <summary>
///     Объект ошибки.
/// </summary>
public class ErrorObject
{
	public ErrorObject(string error)
	{
		Error = error.AssertNotNull();
	}

	/// <summary>
	///     Текст ошибки.
	/// </summary>
	[Required]
	public string Error { get; }
}