using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;
using TaskManagementSystem.Shared.Models;

namespace TaskManagementSystem.Client.Shared.Components.Toasts;

public partial class ToastComponent : IDisposable
{
    [Inject]
    public IToastService? ToastService { get; set; }

    private ICollection<ToastViewModel> toasts = new List<ToastViewModel>();

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        ToastService!.NotifyAdded += OnToastAdded;
        ToastService!.NotifyDeleted += OnToastDeleted;
    }

    public void OnToastAdded(Toast toast)
    {
        ToastViewModel toastViewModel = new(ToastService!, toast);
        toasts.Add(toastViewModel);
        toastViewModel.RemoveAfterTimeout();
        StateHasChanged();
    }
    
    public void OnToastDeleted(Guid id)
    {
        ToastViewModel toastViewModel = toasts.First(x => x.Id == id);
        toasts.Remove(toastViewModel);
        StateHasChanged();
    }
    
    public void Dispose()
    {
        ToastService!.NotifyAdded -= OnToastAdded;
        ToastService!.NotifyDeleted -= OnToastDeleted;
    }
}