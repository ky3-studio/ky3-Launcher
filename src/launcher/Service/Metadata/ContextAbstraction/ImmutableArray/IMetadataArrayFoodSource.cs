using Launcher.Model.Metadata.Food;
using System.Collections.Immutable;

namespace Launcher.Service.Metadata.ContextAbstraction.ImmutableArray;

internal interface IMetadataArrayFoodSource
{
    ImmutableArray<Food> Foods { get; set; }
}
