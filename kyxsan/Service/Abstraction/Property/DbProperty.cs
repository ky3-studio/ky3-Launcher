//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.Property;
using kyxsan.Model.Entity.Database;
using System.Runtime.CompilerServices;

namespace kyxsan.Service.Abstraction.Property;

internal abstract class DbProperty<T> : ObservableObject, IObservableProperty<T>
{
    private bool deferring;

    public abstract T Value { get; set; }

    protected ref bool Deferring
    {
        get => ref deferring;
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        if (Interlocked.Exchange(ref deferring, true))
        {
            throw kyxsanException.InvalidOperation("Already deferring");
        }

        return NotifyPropertyChangedDeferral.Create(this, static self =>
        {
            if (!Interlocked.Exchange(ref self.deferring, false))
            {
                throw kyxsanException.InvalidOperation("Not deferring");
            }

            self.OnPropertyChanged(nameof(Value));
        });
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    protected static string? GetValue(AppDbContext appDbContext, string key)
    {
        // This method is separated to avoid implicit capture of the key
        return appDbContext.Settings.SingleOrDefault(e => e.Key == key)?.Value;
    }

    protected abstract void SetValue(T value);
}