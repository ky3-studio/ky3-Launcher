//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Service;
using System.Collections.Immutable;

namespace kyxsan.Core.Property;

internal sealed partial class PropertyNullableSelectionWrapper<T, TSource> : ObservableObject, IObservableProperty<T?>
    where T : class
    where TSource : notnull
{
    private readonly IProperty<TSource> source;
    private readonly Func<T?, TSource> valueSelector;
    private bool deferring;
    private T? field;

    public PropertyNullableSelectionWrapper(IProperty<TSource> source, ImmutableArray<T> array, Func<T?, TSource> valueSelector, IEqualityComparer<TSource> equalityComparer)
    {
        this.source = source;
        this.valueSelector = valueSelector;

        field = Selection.Initialize(array, source.Value, valueSelector, equalityComparer);
    }

    public T? Value
    {
        get => @field;
        set
        {
            if (Volatile.Read(ref deferring))
            {
                @field = value;
                source.Value = valueSelector(value);
            }
            else
            {
                if (SetProperty(ref @field, value))
                {
                    source.Value = valueSelector(value);
                }
            }
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        if (Interlocked.Exchange(ref deferring, true))
        {
            throw new InvalidOperationException("Already deferring");
        }

        return NotifyPropertyChangedDeferral.Create(this, static self =>
        {
            if (!Interlocked.Exchange(ref self.deferring, false))
            {
                throw new InvalidOperationException("Not deferring");
            }

            self.OnPropertyChanged(nameof(Value));
        });
    }
}