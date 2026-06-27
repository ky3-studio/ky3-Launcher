using Launcher.UI.Xaml.Control;
using Launcher.ViewModel.Inventory;

namespace Launcher.UI.Xaml.View.Page;

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
