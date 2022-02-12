using TaskManagementSystem.Shared.Helpers;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Services.Implementations;

public class ToastService : IToastService
{
    private Dictionary<Guid, Toast> toasts = new();

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
        Toast toast = new("System", description, text);
        AddToast(toast);
    }
    
    public void AddSystemErrorToast(string text)
    {
        Toast toast = new("System", "An error has occurred", text);
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