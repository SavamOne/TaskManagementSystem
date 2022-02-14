using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.ViewModels;

public class ToastViewModel
{
    private readonly Toast toast;
    private readonly IToastService toastService;

    public ToastViewModel(IToastService toastService, Toast toast)
    {
        this.toastService = toastService;
        this.toast = toast;
    }

    public Guid Id => toast.Id;

    public string Author => toast.Author;

    public string Description => toast.Description;

    public string Text => toast.Text;

    public void RemoveAfterTimeout()
    {
        Task.Run(async () =>
        {
            await Task.Delay(5000);
            toastService.RemoveToast(toast.Id);
        });
    }

    public void Remove()
    {
        toastService.RemoveToast(toast.Id);
    }
}