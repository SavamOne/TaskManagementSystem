using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Shared.Components.Modals;

public partial class Modal
{
    private string modalClass = "";

    private string modalDisplay = "none;";
    private bool showBackdrop = false;

    [Parameter]
    public RenderFragment Title { get; set; }

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter]
    public RenderFragment Footer { get; set; }

    public void Open()
    {
        modalDisplay = "block;";
        modalClass = "show";
        showBackdrop = true;

        StateHasChanged();
    }

    public void Close()
    {
        modalDisplay = "none";
        modalClass = "";
        showBackdrop = false;

        StateHasChanged();
    }
}