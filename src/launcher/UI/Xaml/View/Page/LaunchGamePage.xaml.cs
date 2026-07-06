//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Launcher.Service.Game;
using Launcher.UI.Content;
using Launcher.UI.Xaml.Control;
using Launcher.ViewModel.Game;

namespace Launcher.UI.Xaml.View.Page;

[SuppressMessage("", "CA1001")]
internal sealed partial class LaunchGamePage : ScopedPage
{
    private LaunchGameViewModel? _viewModel;
    private CancellationTokenSource? _refreshCts;
    private DispatcherTimer? _configSyncTimer;
    private DispatcherTimer? _gameStateTimer;
    private bool _isSyncingFromDll;
    private bool _injectionToggleChanging = true;

    public LaunchGamePage()
    {
        InitializeComponent();
        Loaded += OnPageLoaded;
        Unloaded += OnPageUnloaded;

        _gameStateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _gameStateTimer.Tick += GameStateTimer_Tick;
        _gameStateTimer.Start();
    }

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        hdrManualToggleInitializing = true;
        HdrManualToggle.IsOn = false;
        hdrManualToggleInitializing = false;

        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            _injectionToggleChanging = false;
        });
    }

    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        if (_gameStateTimer != null)
        {
            _gameStateTimer.Stop();
            _gameStateTimer.Tick -= GameStateTimer_Tick;
            _gameStateTimer = null;
        }

        StopConfigSync();
        _refreshCts?.Cancel();
        _refreshCts?.Dispose();
        _refreshCts = null;
    }

    private void GameStateTimer_Tick(object? sender, object e)
    {
        try
        {
            bool running = GameLifeCycle.IsGameRunningRequiresMainThread();
            KillGameProcessButton.IsEnabled = running;
            LaunchGameButton.IsEnabled = !running;
            ResetGamePathButton.IsEnabled = !running;

            if (running)
            {
                StartConfigSync();
            }
            else
            {
                StopConfigSync();
            }
        }
        catch
        {
        }
    }

    private async void KillGameProcess_Click(object sender, RoutedEventArgs e)
    {
        if (XamlRoot.XamlContext() is { } context)
        {
            ITaskContext taskContext = context.ServiceProvider.GetRequiredService<ITaskContext>();
            await GameLifeCycle.TryKillGameProcessAsync(taskContext).ConfigureAwait(false);
        }
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<LaunchGameViewModel>();

        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is { } options)
        {
            _viewModel = vm;
            RefreshResolution(options);
            StartAutoRefresh();

            LaunchOptions.IsGameRunning.PropertyChanged += OnIsGameRunningChanged;

            bool running = GameLifeCycle.IsGameRunningRequiresMainThread();
            KillGameProcessButton.IsEnabled = running;
            LaunchGameButton.IsEnabled = !running;
            ResetGamePathButton.IsEnabled = !running;
        }
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (_viewModel?.LaunchOptions is { } options)
        {
            RefreshResolution(options);
        }
    }
}
