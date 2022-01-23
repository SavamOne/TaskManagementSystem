using Microsoft.AspNetCore.Components.Forms;
using TaskManagementSystem.Client.Shared.Modals;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Pages;

public partial class Login
{
    private readonly LoginViewModel loginViewModel = new();

    private Modal Modal { get; set; } = new();

    private string? ErrorText { get; set; }

    private async void Callback(EditContext obj)
    {
        ( bool isSuccess, _, string? errorDescription ) = await serverProxy.LoginUserAsync(loginViewModel.GetData());

        if (isSuccess)
        {
            return;
        }

        ErrorText = errorDescription!;
        Modal.Open();
    }
}