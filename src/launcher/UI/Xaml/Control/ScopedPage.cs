//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using Launcher.Service.Navigation;
using Launcher.UI.Content;
using Launcher.ViewModel.Abstraction;

namespace Launcher.UI.Xaml.Control;

[SuppressMessage("", "CA1001")]
internal partial class ScopedPage : Page
{
    private CancellationTokenSource viewCts = new();
    private IServiceScope? scope;
    private bool initialized;
    private volatile bool isDisposed;

    protected ScopedPage()
    {
        NavigationCacheMode = NavigationCacheMode.Disabled;
        Loading += OnLoading;
        Unloaded += OnUnloaded;
    }

    public CancellationToken CancellationToken { get => viewCts.Token; }

    public virtual void UnloadObjectOverride(DependencyObject unloadableObject)
    {
        XamlMarkupHelper.UnloadObject(unloadableObject);
    }

    /// <summary>
    /// Override this method to implement the loading logic.
    /// The page is not attached to the visual tree yet when this method is called.
    /// </summary>
    protected virtual void LoadingOverride()
    {
    }

    /// <summary>
    /// Set <see cref="FrameworkElement.DataContext"/> to an instance of <typeparamref name="TViewModel"/>, which will be retrieved from ServiceProvider
    /// </summary>
    /// <typeparam name="TViewModel">The type of ViewModel</typeparam>
    protected void InitializeDataContext<TViewModel>()
        where TViewModel : class, IViewModel
    {
        ArgumentNullException.ThrowIfNull(scope);

        TViewModel viewModel = scope.ServiceProvider.GetRequiredService<TViewModel>();
        using (viewModel.CriticalSection.Enter())
        {
            viewModel.Resurrect();
            viewModel.CancellationToken = CancellationToken;
            viewModel.DeferContentLoader = new DeferContentLoader(this);

            DataContext = viewModel;
        }
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        // OnNavigatedTo is called before any FrameworkElement event on the page.
        if (e.Parameter is INavigationCompletionSource data)
        {
            NavigationExtraDataSupport.NotifyRecipientAsync(this, data, CancellationToken).SafeForget();
        }
    }

    private void OnLoading(FrameworkElement element, object e)
    {
        isDisposed = false;

        if (!initialized)
        {
            initialized = true;

            XamlContext? context = element.XamlRoot.XamlContext();
            ArgumentNullException.ThrowIfNull(context);
            scope = context.ServiceProvider.CreateScope();

            LoadingOverride();
        }
        else
        {
            // Page returned from navigation cache - create new CTS first, then dispose old
            // This prevents ObjectDisposedException if background code accesses CancellationToken concurrently
            CancellationTokenSource old = viewCts;
            viewCts = new CancellationTokenSource();
            old.Dispose();
            if (DataContext is IViewModel viewModel)
            {
                viewModel.CancellationToken = CancellationToken;
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (isDisposed)
        {
            return;
        }

        isDisposed = true;

        try
        {
            viewCts.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }

        viewCts.Dispose();
        scope?.Dispose();
        scope = null;
    }
}