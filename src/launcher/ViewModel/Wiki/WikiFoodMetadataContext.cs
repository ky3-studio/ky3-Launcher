using Launcher.Model.Metadata.Food;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableArray;
using System.Collections.Immutable;

namespace Launcher.ViewModel.Wiki;

internal sealed class WikiFoodMetadataContext : IMetadataContext,
    IMetadataArrayFoodSource
{
    public ImmutableArray<Food> Foods { get; set; }
}
