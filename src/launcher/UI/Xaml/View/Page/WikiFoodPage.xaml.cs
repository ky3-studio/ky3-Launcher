using Launcher.UI.Xaml.Control;
using Launcher.ViewModel.Wiki;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class WikiFoodPage : ScopedPage
{
    public WikiFoodPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<WikiFoodViewModel>();
    }
}
