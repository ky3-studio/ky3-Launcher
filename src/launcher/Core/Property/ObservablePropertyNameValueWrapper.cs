//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.ExceptionService;
using Launcher.Model;
using Launcher.Service;
using System.Collections.Immutable;

namespace Launcher.Core.Property;

internal sealed partial class ObservablePropertyNameValueWrapper<T> : ObservableObject, IObservableProperty<NameValue<T>?>
    where T : notnull
{
    private readonly IObservableProperty<T> target;
    private readonly ImmutableArray<NameValue<T>> array;
    private bool deferring;
    private bool isExternalSet = true;

    public ObservablePropertyNameValueWrapper(IObservableProperty<T> target, ImmutableArray<NameValue<T>> array)
    {
        this.target = target;
        this.array = array;

        target.WeakPropertyChanged(this, OnWeakTargetValueChanged);
    }

    public NameValue<T>? Value
    {
        get => field ??= Selection.Initialize(array, target.Value);
        set
        {
            if (Volatile.Read(ref deferring))
            {
                field = value;
                if (value is not null)
                {
                    target.Value = value.Value;
                }
            }
            else
            {
                if (SetProperty(ref field, value) && value is not null)
                {
                    Interlocked.Exchange(ref isExternalSet, false);
                    target.Value = value.Value;
                    Interlocked.Exchange(ref isExternalSet, true);
                }
            }
        }
    }

    public INotifyPropertyChangedDeferral GetDeferral()
    {
        if (Interlocked.Exchange(ref deferring, true))
        {
            throw LauncherException.InvalidOperation("Already deferring");
        }

        INotifyPropertyChangedDeferral targetDeferral = target.GetDeferral();
        return NotifyPropertyChangedDeferral.Create(this, targetDeferral, static (self, targetDeferral) =>
        {
            if (!Interlocked.Exchange(ref self.deferring, false))
            {
                throw LauncherException.InvalidOperation("Not deferring");
            }

            self.OnPropertyChanged(nameof(Value));
            using (targetDeferral)
            {
            }
        });
    }

    private static void OnWeakTargetValueChanged(ObservablePropertyNameValueWrapper<T> self, object? sender, PropertyChangedEventArgs e)
    {
        if (Volatile.Read(ref self.isExternalSet))
        {
            self.Value = Selection.Initialize(self.array, self.target.Value);
        }
    }
}