using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Shared.Components.Modals;

public partial class EditFormModal<TItem>
{
    private string modalClass = "";

    private string modalDisplay = "none;";
    private bool showBackdrop = false;

    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public RenderFragment Body { get; set; }

    [Parameter]
    public TItem Item { get; set; }

    [Parameter]
    public Action<TItem>? Submit { get; set; }

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

    private void OnValidSubmit()
    {
        Submit?.Invoke(Item);

        Close();
    }
}