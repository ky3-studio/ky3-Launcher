//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace kyxsan.Core.Property;

internal sealed partial class ObservablePropertyObserver<TSource, T> : ObservableObject, IReadOnlyObservableProperty<T>
{
    private readonly IObservableProperty<TSource> source;
    private readonly Func<TSource, T> converter;
    private T field;

    public ObservablePropertyObserver(IObservableProperty<TSource> source, Func<TSource, T> converter)
    {
        this.source = source;
        this.converter = converter;

        field = converter(source.Value);
        this.source.WeakPropertyChanged(this, OnWeakSourceValueChanged);
    }

    public T Value
    {
        get => @field;
        private set => SetProperty(ref @field, value);
    }

    private static void OnWeakSourceValueChanged(ObservablePropertyObserver<TSource, T> self, object? sender, PropertyChangedEventArgs e)
    {
        self.Value = self.converter(self.source.Value);
    }
}