using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Components;

public partial class LoginComponent
{
    private readonly LoginViewModel loginViewModel = new();

    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    private async void OnLogin()
    {
        var result = await ServerProxy!.LoginAsync(loginViewModel.GetData());

        if (result.IsSuccess)
        {
            return;
        }

        ToastService!.AddSystemErrorToast(result.ErrorDescription!);
    }
}