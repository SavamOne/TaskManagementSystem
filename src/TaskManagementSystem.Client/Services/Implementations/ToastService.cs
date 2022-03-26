using TaskManagementSystem.Client.Resources;
using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Services.Implementations;

public class ToastService : IToastService
{
	private readonly Dictionary<Guid, Toast> toasts = new();

	public event Action<Toast>? NotifyAdded;

	public event Action<Guid>? NotifyDeleted;

	public void AddToast(Toast toast)
	{
		toast.AssertNotNull();

		toasts[toast.Id] = toast;

		NotifyAdded?.Invoke(toast);
	}

	public void AddSystemToast(string description, string text)
	{
		Toast toast = new(LocalizedResources.ToastService_System, description, text);
		AddToast(toast);
	}

	public void AddSystemErrorToast(string text)
	{
		Toast toast = new(LocalizedResources.ToastService_System, LocalizedResources.ToastService_AnErrorHasOccurred, text);
		AddToast(toast);
	}

	public IEnumerable<Toast> GetAllToasts()
	{
		return toasts.Values.ToList();
	}

	public void RemoveToast(Guid id)
	{
		if (toasts.ContainsKey(id))
		{
			toasts.Remove(id);
			NotifyDeleted?.Invoke(id);
		}
	}
}