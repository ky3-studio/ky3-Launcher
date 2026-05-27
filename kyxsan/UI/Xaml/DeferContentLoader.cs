//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using kyxsan.UI.Xaml.Control;

namespace kyxsan.UI.Xaml;

internal sealed class DeferContentLoader : IDeferContentLoader
{
    private readonly WeakReference<FrameworkElement> contentHost = new(default!);

    public DeferContentLoader(FrameworkElement element)
    {
        contentHost.SetTarget(element);
    }

    public DependencyObject? Load(string name)
    {
        if (contentHost.TryGetTarget(out FrameworkElement? element))
        {
            return element.FindName(name) as DependencyObject;
        }

        return default;
    }

    public void Unload(DependencyObject @object)
    {
        if (contentHost.TryGetTarget(out FrameworkElement? element) && element is ScopedPage scopedPage)
        {
            scopedPage.UnloadObjectOverride(@object);
        }
        else
        {
            XamlMarkupHelper.UnloadObject(@object);
        }
    }
}