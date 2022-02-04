using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Shared.Components.Modals;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Shared.Components;

public partial class LoginComponent
{
    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    private Modal Modal { get; set; } = new();
    
    private readonly LoginViewModel loginViewModel = new();

    private string? ErrorText { get; set; }

    private async void OnLogin()
    {
        ( bool isSuccess, _, string? errorDescription ) = await ServerProxy!.LoginUserAsync(loginViewModel.GetData());

        if (isSuccess)
        {
            return;
        }

        ErrorText = errorDescription!;
        Modal.Open();
    }
}