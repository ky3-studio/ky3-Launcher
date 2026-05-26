//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using JetBrains.Annotations;

namespace kyxsan.Core.Property;

internal static class ReadOnlyProperty
{
    extension<T>(IReadOnlyProperty<T> property)
    {
        public T Get()
        {
            return property.Value;
        }
    }

    extension<T>(IReadOnlyObservableProperty<T> source)
    {
        public IReadOnlyObservableProperty<T> Debug(string name)
        {
            return new ReadOnlyObservablePropertyDebug<T>(source, name);
        }

        public IReadOnlyObservableProperty<T> WithValueChangedCallback([RequireStaticDelegate] Action<T> callback)
        {
            return new ReadOnlyPropertyValueChangedCallbackWrapper<T>(source, callback);
        }

        public IReadOnlyObservableProperty<T> WithValueChangedCallback<TState>([RequireStaticDelegate] Action<T, TState> callback, TState state)
        {
            return new ReadOnlyPropertyValueChangedCallbackWrapper<T, TState>(source, callback, state);
        }
    }
}