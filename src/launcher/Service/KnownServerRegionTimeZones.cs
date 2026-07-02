using Launcher.Model;
using Launcher.Web.Hoyolab;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Launcher.Service;

internal static class KnownServerRegionTimeZones
{
    public static ImmutableArray<NameValue<TimeSpan>> Value
    {
        get
        {
            Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
            return !field.IsDefault ? field : field =
            [
                new(SH.ServiceAppOptionsCalendarServerTimeZoneCommon, ServerRegionTimeZone.CommonOffset),
                new(SH.ServiceAppOptionsCalendarServerTimeZoneAmerica, ServerRegionTimeZone.AmericaServerOffset),
                new(SH.ServiceAppOptionsCalendarServerTimeZoneEurope, ServerRegionTimeZone.EuropeServerOffset),
            ];
        }
    }
}
