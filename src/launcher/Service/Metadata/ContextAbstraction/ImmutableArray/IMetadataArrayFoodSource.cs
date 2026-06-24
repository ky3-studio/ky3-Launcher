using kyxsan.Model.Metadata.Food;
using System.Collections.Immutable;

namespace kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;

internal interface IMetadataArrayFoodSource
{
    ImmutableArray<Food> Foods { get; set; }
}
