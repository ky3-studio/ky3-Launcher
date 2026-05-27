//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Core.Setting;
using kyxsan.UI.Input.HotKey;
using kyxsan.UI.Input.LowLevel;

namespace kyxsan.ViewModel.Setting;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingHotKeyViewModel : Abstraction.ViewModel
{
    [GeneratedConstructor]
    public partial SettingHotKeyViewModel(IServiceProvider serviceProvider);

    public static int WebView2VideoFastForwardOrRewindSeconds
    {
        get => LocalSetting.Get(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, 5);
        set => LocalSetting.Set(SettingKeys.WebView2VideoFastForwardOrRewindSeconds, value);
    }

    public partial LowLevelKeyOptions LowLevelKeyOptions { get; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial HotKeyOptions HotKeyOptions { get; }
}