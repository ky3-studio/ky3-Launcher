using kyxsan.Model.Metadata.Food;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Wiki;

internal sealed class WikiFoodMetadataContext : IMetadataContext,
    IMetadataArrayFoodSource
{
    public ImmutableArray<Food> Foods { get; set; }
}
