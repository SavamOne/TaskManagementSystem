using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Resources;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Components;

public partial class ChangePasswordComponent : ComponentBase
{
    private readonly UpdateUserPasswordViewModel passwordViewModel = new();

    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    private async Task ChangePassword()
    {
        var result = await ServerProxy!.ChangeUserPasswordAsync(passwordViewModel.GetRequest());
        if (result.IsSuccess)
        {
            ToastService!.AddSystemToast(LocalizedResources.ChangePasswordComponent_PasswordChange, LocalizedResources.ChangePasswordComponent_PasswordSuccessfullyChanged);
            return;
        }

        ToastService!.AddSystemErrorToast(result.ErrorDescription!);
    }
}