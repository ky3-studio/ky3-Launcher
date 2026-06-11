using kyxsan.Model.Metadata.Item;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Calendar;

internal sealed class CalendarMaterial
{
    public required Material Inner { get; init; }

    public required ImmutableArray<CalendarItem> Items { get; init; }

    internal required RotationalMaterialIdEntry InnerEntry { get; init; }
}
