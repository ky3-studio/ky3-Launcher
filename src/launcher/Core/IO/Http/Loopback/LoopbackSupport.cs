//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Win32;

namespace Launcher.Core.IO.Http.Loopback;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LoopbackSupport : ObservableObject
{
    private readonly LauncherNativeLoopbackSupport native;
    private readonly string LauncherContainerStringSid;

    public LoopbackSupport(IServiceProvider serviceProvider)
    {
        native = LauncherNative.Instance.MakeLoopbackSupport();
        try
        {
            if (!native.IsPublicFirewallEnabled)
            {
                IsLoopbackEnabled = false;
                LauncherContainerStringSid = string.Empty;
                return;
            }

            IsLoopbackEnabled = native.IsEnabled(LauncherRuntime.FamilyName, out string? sid);
            LauncherContainerStringSid = sid ?? string.Empty;
        }
        catch
        {
            IsLoopbackEnabled = false;
            LauncherContainerStringSid = string.Empty;
        }

#pragma warning disable SA1116, SA1117
        SentrySdk.ConfigureScope(static (scope, state) =>
        {
            Dictionary<string, object> loopback = new()
            {
                ["Enabled"] = state.Enabled,
                ["Sid"] = state.Sid,
            };

            scope.Contexts["Loopback"] = loopback;
        }, (Sid: LauncherContainerStringSid, Enabled: IsLoopbackEnabled));
#pragma warning restore SA1116, SA1117
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanEnableLoopback))]
    public partial bool IsLoopbackEnabled { get; private set; }

    public bool CanEnableLoopback
    {
        get => LauncherRuntime.IsProcessElevated && !IsLoopbackEnabled && !string.IsNullOrEmpty(LauncherContainerStringSid);
    }

    public void EnableLoopback()
    {
        if (!CanEnableLoopback)
        {
            return;
        }

        native.Enable(LauncherContainerStringSid);
        IsLoopbackEnabled = true;
    }
}