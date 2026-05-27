using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.GachaLog;

namespace kyxsan.UI.Xaml.View.Page;

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
