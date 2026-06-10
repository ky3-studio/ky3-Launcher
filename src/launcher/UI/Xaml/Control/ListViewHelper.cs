//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace kyxsan.UI.Xaml.Control;

[SuppressMessage("", "SH001")]
[DependencyProperty<object>("ScrollToTopAssociatedObject", PropertyChangedCallbackName = nameof(OnScrollToTopAssociatedObjectChanged), IsAttached = true, TargetType = typeof(ListView))]
public sealed partial class ListViewHelper
{
    private static void OnScrollToTopAssociatedObjectChanged(DependencyObject dp, DependencyPropertyChangedEventArgs args)
    {
        ListView listView = (ListView)dp;
        if (listView is not { IsLoaded: true, ItemsPanelRoot: { IsLoaded: true } panel })
        {
            return;
        }

        LayoutUpdatedHandler handler = new(listView, args);
        panel.LayoutUpdated += handler.Handle;
    }

    private sealed class LayoutUpdatedHandler
    {
        private readonly WeakReference<ListView> weakView;
        private readonly WeakReference<DependencyPropertyChangedEventArgs> weakArgs;

        public LayoutUpdatedHandler(ListView listView, DependencyPropertyChangedEventArgs args)
        {
            weakView = new(listView);
            weakArgs = new(args);
        }

        public void Handle(object? sender, object e)
        {
            if (!weakArgs.TryGetTarget(out DependencyPropertyChangedEventArgs? args))
            {
                return;
            }

            if (args.OldValue != args.NewValue)
            {
                if (weakView.TryGetTarget(out ListView? view))
                {
                    view.SmoothScrollIntoViewWithIndexAsync(0).SafeForget();
                    view.ItemsPanelRoot.LayoutUpdated -= Handle;
                }
            }
        }
    }
}