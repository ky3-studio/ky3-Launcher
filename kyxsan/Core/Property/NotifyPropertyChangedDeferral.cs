//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Property;

internal static class NotifyPropertyChangedDeferral
{
    public static NotifyPropertyChangedDeferral<T> Create<T>(T source, Action<T> raisePropertyChanged)
        where T : INotifyPropertyChanged
    {
        return new(source, raisePropertyChanged);
    }

    public static NotifyPropertyChangedDeferral<T, TState> Create<T, TState>(T source, TState state, Action<T, TState> raisePropertyChanged)
        where T : INotifyPropertyChanged
    {
        return new(source, state, raisePropertyChanged);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class NotifyPropertyChangedDeferral<T> : INotifyPropertyChangedDeferral
    where T : INotifyPropertyChanged
{
    private readonly T source;
    private readonly Action<T> raisePropertyChanged;

    public NotifyPropertyChangedDeferral(T source, Action<T> raisePropertyChanged)
    {
        this.source = source;
        this.raisePropertyChanged = raisePropertyChanged;
    }

    public void Dispose()
    {
        raisePropertyChanged(source);
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class NotifyPropertyChangedDeferral<T, TState> : INotifyPropertyChangedDeferral
    where T : INotifyPropertyChanged
{
    private readonly T source;
    private readonly TState state;
    private readonly Action<T, TState> raisePropertyChanged;

    public NotifyPropertyChangedDeferral(T source, TState state, Action<T, TState> raisePropertyChanged)
    {
        this.source = source;
        this.raisePropertyChanged = raisePropertyChanged;
        this.state = state;
    }

    public void Dispose()
    {
        raisePropertyChanged(source, state);
    }
}