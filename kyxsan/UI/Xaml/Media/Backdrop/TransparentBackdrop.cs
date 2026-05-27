//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace kyxsan.UI.Xaml.Media.Backdrop;

internal sealed partial class TransparentBackdrop : SystemBackdrop, IBackdropNeedEraseBackground
{
    private readonly Color tintColor;

    private Windows.UI.Composition.CompositionColorBrush? brush;
    private Windows.UI.Composition.Compositor? compositor;
    private object? compositorLock;

    public TransparentBackdrop()
        : this(Colors.Transparent)
    {
    }

    public TransparentBackdrop(Color tintColor)
    {
        this.tintColor = tintColor;
    }

    internal Windows.UI.Composition.Compositor Compositor
    {
        // ReSharper disable once InconsistentlySynchronizedField
        get => LazyInitializer.EnsureInitialized(ref compositor, ref compositorLock, () =>
        {
            DispatcherQueue.EnsureSystemDispatcherQueue();
            return new();
        });
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        brush ??= Compositor.CreateColorBrush(tintColor);
        target.SystemBackdrop = brush;
    }

    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop target)
    {
        target.SystemBackdrop = null;

        if (compositorLock is not null)
        {
            lock (compositorLock)
            {
                compositor?.Dispose();
            }
        }
    }
}