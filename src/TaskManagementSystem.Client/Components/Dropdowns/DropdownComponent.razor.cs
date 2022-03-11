using Microsoft.AspNetCore.Components;

namespace TaskManagementSystem.Client.Components.Dropdowns;

public partial class DropdownComponent<TItem>
{
    private string disabledClass = string.Empty;

    private string showClass = string.Empty;

    [Parameter]
    public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

    [Parameter]
    public TItem? SelectedItem { get; set; }

    [Parameter]
    public Func<TItem?, string> ItemToStringFunc { get; set; } = item => item?.ToString()!;

    [Parameter]
    public Action<TItem>? ItemSelectedFunc { get; set; }

    [Parameter]
    public bool Disabled
    {
        get => disabledClass != string.Empty;
        set => disabledClass = value ? "disabled" : string.Empty;
    }

    private bool ShowDropdown
    {
        get => showClass != string.Empty;
        set
        {
            if (Disabled)
            {
                return;
            }

            showClass = value ? "show" : string.Empty;
        }
    }

    protected override void OnParametersSet()
    {
        SelectedItem ??= Items.FirstOrDefault();
    }

    private void OnItemSelected(TItem item)
    {
        if (Disabled)
        {
            return;
        }

        ShowDropdown = false;

        SelectedItem = item;
        ItemSelectedFunc?.Invoke(item);
    }
}