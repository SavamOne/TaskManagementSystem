using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Shared.Components.Dropdowns;

public partial class DropdownComponent<TItem>
{
    [Parameter]
    public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    [Parameter]
    public TItem? SelectedItem { get; set; }

    [Parameter]
    public Func<TItem?, string> ItemToStringFunc { get; set; } = item => item?.ToString()!;

    [Parameter]
    public Action<TItem>? ItemSelectedFunc { get; set; }

    private string showClass = string.Empty;

    private bool ShowDropdown
    {
        get => showClass != string.Empty;
        set => showClass = value ? "show" : string.Empty;
    }

    protected override void OnParametersSet()
    {
        SelectedItem ??= Items.FirstOrDefault();
    }

    private void OnItemSelected(TItem item)
    {
        ShowDropdown = false;

        SelectedItem = item;
        ItemSelectedFunc?.Invoke(item);
    }
}