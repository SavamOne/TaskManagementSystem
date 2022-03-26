using Microsoft.AspNetCore.Components;
using TaskManagementSystem.Client.Proxies;

namespace TaskManagementSystem.Client.Pages;

public partial class Profile
{
	[Inject]
	public ServerProxy? ServerProxy { get; set; }

	private async Task Logout()
	{
		await ServerProxy!.LogoutAsync();
	}
}