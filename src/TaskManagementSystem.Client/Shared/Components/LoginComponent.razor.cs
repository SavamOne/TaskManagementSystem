using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Shared.Components.Modals;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Shared.Components;

public partial class LoginComponent
{

    private readonly LoginViewModel loginViewModel = new();

    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    private Modal Modal { get; set; } = new();

    private string? ErrorText { get; set; }

    private async void OnLogin()
    {
        var result = await ServerProxy!.LoginAsync(loginViewModel.GetData());

        if (result.IsSuccess)
        {
            return;
        }

        ErrorText = result.ErrorDescription!;
        Modal.Open();
    }
}