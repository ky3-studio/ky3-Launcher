//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using System.Runtime.CompilerServices;

namespace kyxsan.UI.Xaml;

internal static class FrameworkElementExtension
{
    extension(FrameworkElement frameworkElement)
    {
        /// <summary>
        /// Make properties below false:
        /// <code>
        /// * AllowFocusOnInteraction
        /// * IsDoubleTapEnabled
        /// * IsHitTestVisible
        /// * IsHoldingEnabled
        /// * IsRightTapEnabled
        /// * IsTabStop
        /// </code>
        /// </summary>
        public void DisableInteraction()
        {
            frameworkElement.AllowFocusOnInteraction = false;
            frameworkElement.IsDoubleTapEnabled = false;
            frameworkElement.IsHitTestVisible = false;
            frameworkElement.IsHoldingEnabled = false;
            frameworkElement.IsRightTapEnabled = false;
            frameworkElement.IsTabStop = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T? DataContext<T>()
            where T : class
        {
            return frameworkElement.DataContext as T;
        }

        public void InitializeDataContext<TDataContext>(IServiceProvider serviceProvider)
            where TDataContext : class
        {
            try
            {
                frameworkElement.DataContext = serviceProvider.GetRequiredService<TDataContext>();
                (frameworkElement as IDataContextInitialized)?.OnDataContextInitialized(serviceProvider);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}