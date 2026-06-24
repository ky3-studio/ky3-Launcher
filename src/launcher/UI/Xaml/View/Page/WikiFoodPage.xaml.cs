using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.Wiki;

namespace kyxsan.UI.Xaml.View.Page;

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
