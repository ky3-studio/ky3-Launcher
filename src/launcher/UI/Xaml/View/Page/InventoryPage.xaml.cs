using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.Inventory;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class InventoryPage : ScopedPage
{
    public InventoryPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<InventoryViewModel>();
    }
}
