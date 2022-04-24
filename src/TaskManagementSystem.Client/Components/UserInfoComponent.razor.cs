using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;
using TaskManagementSystem.Client.Resources;
using TaskManagementSystem.Client.Services;
using TaskManagementSystem.Client.ViewModels;

namespace TaskManagementSystem.Client.Components;

public partial class UserInfoComponent
{
	private UserInfoViewModel userInfoViewModel = new();

	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	[Inject]
	public IToastService? ToastService { get; set; }
	
	[Inject]
	public IUserUpdateService? UserUpdateService { get; set; }

	protected override async Task OnInitializedAsync()
	{
		var result = await ServerProxy!.GetUserInfoAsync();

		if (!result.IsSuccess)
		{
			ToastService!.AddSystemErrorToast(result.ErrorDescription!);
			return;
		}

		userInfoViewModel = new UserInfoViewModel(result.Value!);
	}

	private async Task EditInfoAsync()
	{
		var result = await ServerProxy!.EditUserInfoAsync(userInfoViewModel.GetEditInfoRequest());

		if (result.IsSuccess)
		{
			ToastService!.AddSystemToast(LocalizedResources.UserInfoComponent_AboutMe, LocalizedResources.UserInfoComponent_AboutSuccessfullyChanged);
			UserUpdateService!.SetUpdatedInfo(result.Value!);
			return;
		}

		ToastService!.AddSystemErrorToast(result.ErrorDescription!);
	}
}