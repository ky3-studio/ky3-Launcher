//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Property;

internal sealed partial class ObservablePropertyWithConditionalSetMethod<T, TState> : IObservableProperty<T>
{
    private readonly IObservableProperty<T> source;
    private readonly Func<T, TState, bool> condition;
    private readonly TState state;

    public ObservablePropertyWithConditionalSetMethod(IObservableProperty<T> source, Func<T, TState, bool> condition, TState state)
    {
        this.source = source;
        this.condition = condition;
        this.state = state;
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
        set
        {
            if (condition(value, state))
            {
                source.Value = value;
            }
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        return source.GetDeferral();
    }
}