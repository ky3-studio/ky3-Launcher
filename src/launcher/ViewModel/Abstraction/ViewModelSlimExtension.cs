//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using kyxsan.UI.Xaml;
using WinRT;

namespace kyxsan.ViewModel.Abstraction;

internal static class ViewModelSlimExtension
{
    extension(FrameworkElement frameworkElement)
    {
        public void InitializeViewModelSlim<TDataContext>(IServiceProvider serviceProvider)
            where TDataContext : ViewModelSlim
        {
            frameworkElement.Unloaded += OnFrameworkElementUnloaded;
            frameworkElement.InitializeDataContext<TDataContext>(serviceProvider);
        }
    }

    private static void OnFrameworkElementUnloaded(object sender, RoutedEventArgs e)
    {
        FrameworkElement frameworkElement = sender.As<FrameworkElement>();
        frameworkElement.Unloaded -= OnFrameworkElementUnloaded;
        frameworkElement.DataContext<ViewModelSlim>()?.IsViewUnloaded.Value = true;
    }
}