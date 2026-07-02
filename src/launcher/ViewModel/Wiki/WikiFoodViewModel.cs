using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Logging;
using Launcher.Model.Metadata.Food;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.UI.Xaml.Control.AutoSuggestBox;
using Launcher.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Launcher.ViewModel.Wiki;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiFoodViewModel : Abstraction.ViewModel
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    public WikiFoodViewModel(IServiceProvider serviceProvider)
    {
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public IAdvancedCollectionView<Food>? Foods
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentFoodChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentFoodChanged);
        }
    }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<Food>? InventoryFoods { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        WikiFoodMetadataContext context = await metadataService.GetContextAsync<WikiFoodMetadataContext>(token).ConfigureAwait(false);
        ImmutableArray<Food> foods = [.. context.Foods.OrderByDescending(f => f.Rank).ThenBy(f => f.Id)];
        SearchData searchData = SearchData.CreateForWikiFood(foods);

        IAdvancedCollectionView<Food> foodsView;
        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            foodsView = foods.AsAdvancedCollectionView();
        }

        await taskContext.SwitchToMainThreadAsync();
        token.ThrowIfCancellationRequested();

        SearchData = searchData;
        Foods = foodsView;
        Foods.MoveCurrentToFirst();
        InventoryFoods = new([.. foods]);

        return true;
    }

    private void OnCurrentFoodChanged(object? sender, object? e)
    {
        OnPropertyChanged(nameof(Foods));
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter foods", "WikiFoodViewModel.Command"));

        if (Foods is null)
        {
            return;
        }

        Foods.Filter = FoodFilter.Compile(SearchData);

        if (Foods.CurrentItem is null)
        {
            Foods.MoveCurrentToFirst();
        }
    }
}
