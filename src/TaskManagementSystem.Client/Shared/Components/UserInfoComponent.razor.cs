using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Resources;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Shared.Components;

public partial class UserInfoComponent
{
    private UserInfoViewModel userInfoViewModel = new();

    [Inject]
    public ServerProxy? ServerProxy { get; set; }

    [Inject]
    public IToastService? ToastService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var result = await ServerProxy!.GetUserInfoAsync();

        if (!result.IsSuccess)
        {
            ToastService!.AddSystemErrorToast(result.ErrorDescription!);
            return;
        }

        userInfoViewModel.SetInfo(result.Value!);
    }

    private async Task ChangeInfo()
    {
        var result = await ServerProxy!.ChangeUserInfoAsync(userInfoViewModel.GetRequest());

        if (result.IsSuccess)
        {
            ToastService!.AddSystemToast(LocalizedResources.UserInfoComponent_AboutMe, LocalizedResources.UserInfoComponent_AboutSuccessfullyChanged);
            return;
        }

        ToastService!.AddSystemErrorToast(result.ErrorDescription!);
    }
}