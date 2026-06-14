//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using kyxsan.Service.Navigation;
using kyxsan.UI.Content;
using kyxsan.ViewModel.Abstraction;

namespace kyxsan.UI.Xaml.Control;

[SuppressMessage("", "CA1001")]
internal partial class ScopedPage : Page
{
    private CancellationTokenSource viewCts = new();
    private IServiceScope? scope;
    private bool initialized;

    protected ScopedPage()
    {
        NavigationCacheMode = NavigationCacheMode.Enabled;
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
            // Page returned from navigation cache - dispose old CTS and provide fresh one
            viewCts.Dispose();
            viewCts = new CancellationTokenSource();
            if (DataContext is IViewModel viewModel)
            {
                viewModel.CancellationToken = CancellationToken;
            }
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        // Cancel in-flight async operations when page leaves visual tree
        // Do NOT dispose - cached page code may still read CancellationToken
        viewCts.Cancel();
    }
}