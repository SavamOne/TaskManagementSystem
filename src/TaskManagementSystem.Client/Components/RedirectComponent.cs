using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Components;

public class RedirectComponent : ComponentBase
{
    [Inject]
    public NavigationManager NavigationManager { get; set; } = null!;

    [Parameter]
    public string Uri { get; set; } = "Login";

    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo(Uri);
    }
}