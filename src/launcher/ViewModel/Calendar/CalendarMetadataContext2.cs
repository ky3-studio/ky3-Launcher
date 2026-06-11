using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Primitive;

namespace kyxsan.ViewModel.Calendar;

internal sealed class CalendarMetadataContext2
{
    public required CalendarMetadataContext MetadataContext { get; init; }

    public required ILookup<MonthAndDay, Avatar> AvatarBirthdays { get; init; }

    public required ILookup<MaterialId, CalendarItem> MaterialItems { get; init; }
}
