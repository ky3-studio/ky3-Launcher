using Launcher.UI.Xaml.Control;
using Launcher.ViewModel.GachaLog;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class GachaLogPage : ScopedPage
{
    public GachaLogPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<GachaLogViewModel>();
    }
}
