using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Services;

public interface IToastService
{
    event Action<Toast>? NotifyAdded;

    event Action<Guid>? NotifyDeleted;

    void AddToast(Toast toast);

    void AddSystemToast(string description, string text);

    void AddSystemErrorToast(string text);

    IEnumerable<Toast> GetAllToasts();

    void RemoveToast(Guid id);
}