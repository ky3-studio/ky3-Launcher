//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Property;

internal sealed partial class ReadOnlyPropertyValueChangedCallbackWrapper<T> : IReadOnlyObservableProperty<T>
{
    private readonly IReadOnlyObservableProperty<T> source;
    private readonly Action<T> callback;

    public ReadOnlyPropertyValueChangedCallbackWrapper(IReadOnlyObservableProperty<T> source, Action<T> callback)
    {
        this.source = source;
        this.callback = callback;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
    }

    private static void OnWeakSourceValueChanged(ReadOnlyPropertyValueChangedCallbackWrapper<T> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value);
    }
}

internal sealed partial class ReadOnlyPropertyValueChangedCallbackWrapper<T, TState> : IReadOnlyObservableProperty<T>
{
    private readonly IReadOnlyObservableProperty<T> source;
    private readonly Action<T, TState> callback;
    private readonly TState state;

    public ReadOnlyPropertyValueChangedCallbackWrapper(IReadOnlyObservableProperty<T> source, Action<T, TState> callback, TState state)
    {
        this.source = source;
        this.callback = callback;
        this.state = state;

        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => source.PropertyChanged += value;
        remove => source.PropertyChanged -= value;
    }

    public T Value
    {
        get => source.Value;
    }

    private static void OnWeakSourceValueChanged(ReadOnlyPropertyValueChangedCallbackWrapper<T, TState> self, object? sender, PropertyChangedEventArgs e)
    {
        self.callback(self.source.Value, self.state);
    }
}