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

internal sealed partial class SafeMicaBackdrop : SystemBackdrop
{
    private readonly ConcurrentDictionary<ICompositionSupportsSystemBackdrop, MicaController> controllers = [];
    private readonly MicaKind kind;

    public SafeMicaBackdrop(MicaKind kind = MicaKind.Base)
    {
        this.kind = kind;
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(target, xamlRoot);

        if (controllers.TryRemove(target, out MicaController? existing))
        {
            existing.RemoveSystemBackdropTarget(target);
            existing.Dispose();
        }

        try
        {
            MicaController newController = new() { Kind = kind };
            newController.AddSystemBackdropTarget(target);
            newController.SetSystemBackdropConfiguration(GetDefaultSystemBackdropConfiguration(target, xamlRoot));
            controllers.TryAdd(target, newController);
        }
        catch (ArgumentException)
        {
        }
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        base.OnTargetDisconnected(target);

        if (controllers.TryRemove(target, out MicaController? controller))
        {
            controller.RemoveSystemBackdropTarget(target);
            controller.Dispose();
        }
    }
}
