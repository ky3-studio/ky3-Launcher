using Launcher.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Launcher.ViewModel.Calendar;

internal sealed partial class CalendarDay : IPropertyValuesProvider
{
    public required DateTimeOffset Date { get; init; }

    public required int DayInMonth { get; init; }

    public required string DayName { get; init; }

    public required ImmutableArray<Model.Item> BirthDayAvatars { get; init; }

    public required ImmutableArray<CalendarMaterial> Materials { get; init; }
}
