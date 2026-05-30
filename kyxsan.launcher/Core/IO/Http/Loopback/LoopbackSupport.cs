//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Win32;

namespace kyxsan.Core.IO.Http.Loopback;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LoopbackSupport : ObservableObject
{
    private readonly kyxsanNativeLoopbackSupport native;
    private readonly string kyxsanContainerStringSid;

    public LoopbackSupport(IServiceProvider serviceProvider)
    {
        native = kyxsanNative.Instance.MakeLoopbackSupport();
        try
        {
            if (!native.IsPublicFirewallEnabled)
            {
                IsLoopbackEnabled = false;
                kyxsanContainerStringSid = string.Empty;
                return;
            }

            IsLoopbackEnabled = native.IsEnabled(kyxsanRuntime.FamilyName, out string? sid);
            kyxsanContainerStringSid = sid ?? string.Empty;
        }
        catch
        {
            IsLoopbackEnabled = false;
            kyxsanContainerStringSid = string.Empty;
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
        }, (Sid: kyxsanContainerStringSid, Enabled: IsLoopbackEnabled));
#pragma warning restore SA1116, SA1117
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanEnableLoopback))]
    public partial bool IsLoopbackEnabled { get; private set; }

    public bool CanEnableLoopback
    {
        get => kyxsanRuntime.IsProcessElevated && !IsLoopbackEnabled && !string.IsNullOrEmpty(kyxsanContainerStringSid);
    }

    public void EnableLoopback()
    {
        if (!CanEnableLoopback)
        {
            return;
        }

        native.Enable(kyxsanContainerStringSid);
        IsLoopbackEnabled = true;
    }
}