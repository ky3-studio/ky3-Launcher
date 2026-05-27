//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Collections.Concurrent;

namespace kyxsan.UI.Xaml.Media.Backdrop;

// https://github.com/microsoft/microsoft-ui-xaml/blob/winui3/release/1.5-stable/controls/dev/Materials/DesktopAcrylicBackdrop/DesktopAcrylicBackdrop.cpp
internal sealed partial class InputActiveDesktopAcrylicBackdrop : SystemBackdrop
{
    private readonly ConcurrentDictionary<ICompositionSupportsSystemBackdrop, DesktopAcrylicController> controllers = [];

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(target, xamlRoot);

        if (controllers.TryRemove(target, out DesktopAcrylicController? existing))
        {
            existing.RemoveSystemBackdropTarget(target);
            existing.Dispose();
        }

        try
        {
            SystemBackdropConfiguration configuration = GetDefaultSystemBackdropConfiguration(target, xamlRoot);
            configuration.IsInputActive = true;

            DesktopAcrylicController newController = new();
            newController.AddSystemBackdropTarget(target);
            newController.SetSystemBackdropConfiguration(configuration);
            controllers.TryAdd(target, newController);
        }
        catch (ArgumentException)
        {
        }
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        base.OnTargetDisconnected(target);

        if (controllers.TryRemove(target, out DesktopAcrylicController? controller))
        {
            controller.RemoveSystemBackdropTarget(target);
            controller.Dispose();
        }
    }
}