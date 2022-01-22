using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Shared;

public class RedirectComponent : ComponentBase
{
    [Inject] public NavigationManager NavigationManager { get; set; }

    [Parameter] public string Uri { get; set; } = "Login";
    
    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo(Uri);
    }
}