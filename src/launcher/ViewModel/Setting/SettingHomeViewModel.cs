//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Model;
using Launcher.Service;
using Launcher.Web.Hoyolab;

namespace Launcher.ViewModel.Setting;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingHomeViewModel : Abstraction.ViewModel
{
    [GeneratedConstructor]
    public partial SettingHomeViewModel(IServiceProvider serviceProvider);

    public partial AppOptions AppOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public NameValue<Region>? SelectedRegion
    {
        get => field ??= Selection.Initialize(AppOptions.LazyRegions, AppOptions.Region.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.Region.Value = value.Value;
            }
        }
    }

    public NameValue<TimeSpan>? SelectedCalendarServerTimeZone
    {
        get => field ??= Selection.Initialize(AppOptions.LazyCalendarServerTimeZoneOffsets, AppOptions.CalendarServerTimeZoneOffset.Value);
        set
        {
            if (SetProperty(ref field, value) && value is not null)
            {
                AppOptions.CalendarServerTimeZoneOffset.Value = value.Value;
            }
        }
    }
}
