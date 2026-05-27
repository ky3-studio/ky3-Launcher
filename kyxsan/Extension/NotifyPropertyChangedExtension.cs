//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.Helpers;
using JetBrains.Annotations;

namespace kyxsan.Extension;

internal static class NotifyPropertyChangedExtension
{
    extension(INotifyPropertyChanged source)
    {
        public WeakEventListener<TInstance, object?, PropertyChangedEventArgs> WeakPropertyChanged<TInstance>(TInstance instance, [RequireStaticDelegate] Action<TInstance, object?, PropertyChangedEventArgs> callback)
            where TInstance : class
        {
            WeakEventListener<TInstance, object?, PropertyChangedEventArgs> weakEvent = new(instance)
            {
                OnEventAction = callback,
                OnDetachAction = listener => source.PropertyChanged -= listener.OnEvent,
            };

            source.PropertyChanged += weakEvent.OnEvent;
            return weakEvent;
        }
    }
}