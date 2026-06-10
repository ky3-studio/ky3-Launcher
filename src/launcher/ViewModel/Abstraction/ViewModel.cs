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
using kyxsan.UI.Xaml;
using kyxsan.Win32.Foundation;

namespace kyxsan.ViewModel.Abstraction;

internal abstract partial class ViewModel : ObservableObject, IViewModel, IDisposable
{
    private bool initializing;

    [ObservableProperty]
    public partial bool IsInitialized { get; protected set; }

    public CancellationToken CancellationToken { get; set; }

    [field: MaybeNull]
    public SemaphoreSlim CriticalSection { get => field ??= new(1); private set; }

    public IDeferContentLoader? DeferContentLoader { get; set; }

    [field: MaybeNull]
    public IProperty<bool> IsViewUnloaded { get => field ??= Property.Create(false); protected set; }

    protected TaskCompletionSource<bool> Initialization { get; set; } = new();

    public virtual void Dispose()
    {
        CriticalSection.Dispose();
    }

    public void Resurrect()
    {
        IsViewUnloaded.Value = false;
        Initialization = new();
    }

    public void Uninitialize()
    {
        try
        {
            IsInitialized = false;
        }
        catch (Exception ex) when (ex.HResult is HRESULT.E_UNEXPECTED)
        {
        }

        // Must be called before setting IsViewUnloaded to true, we sometime set property value to null in it
        // BindableCustomPropertyProvider will check IsViewUnloaded before SetProperty
        UninitializeOverride();

        IsViewUnloaded.Value = true;
        DeferContentLoader = default;
    }

    [Command("LoadCommand")]
    protected virtual async Task LoadAsync()
    {
        try
        {
            if (Interlocked.Exchange(ref initializing, true))
            {
                await Initialization.Task.ConfigureAwait(false);
                return;
            }

            IsInitialized = await LoadOverrideAsync(CancellationToken).ConfigureAwait(true);
            Initialization.TrySetResult(IsInitialized);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            Volatile.Write(ref initializing, false);
        }
    }

    protected virtual ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return ValueTask.FromResult(true);
    }

    protected virtual void UninitializeOverride()
    {
    }

    protected async ValueTask<IDisposable> EnterCriticalSectionAsync()
    {
        ThrowIfViewDisposed();
        IDisposable disposable = await CriticalSection.EnterAsync(CancellationToken).ConfigureAwait(false);
        ThrowIfViewDisposed();
        return disposable;
    }

    protected void MakeSubViewModel(ReadOnlySpan<ViewModel> subViewModels)
    {
        foreach (ViewModel subViewModel in subViewModels)
        {
            subViewModel.CancellationToken = CancellationToken;
            subViewModel.CriticalSection = CriticalSection;
            subViewModel.IsViewUnloaded = IsViewUnloaded;
        }
    }

    private void ThrowIfViewDisposed()
    {
        if (IsViewUnloaded.Value)
        {
            kyxsanException.OperationCanceled(SH.ViewModelViewDisposedOperationCancel);
        }
    }
}