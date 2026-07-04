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
using Launcher.Service.Game;
using Launcher.Service.Game.Launching;
using Launcher.ViewModel.Game;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LaunchGamePage
{
    private void StartAutoRefresh()
    {
        _refreshCts = new CancellationTokenSource();
        CancellationToken token = _refreshCts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(2000, token).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                if (!LaunchOptions.IsGameRunning.Value)
                {
                    continue;
                }

                try
                {
                    Microsoft.UI.Dispatching.DispatcherQueue? queue = DispatcherQueue;
                    if (queue is null)
                    {
                        break;
                    }

                    queue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        if (_viewModel?.LaunchOptions is { } options)
                        {
                            options.RefreshFromRegistry();
                        }
                    });
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    SentrySdk.CaptureException(ex);
                }
            }
        }, token);
    }

    private void OnIsGameRunningChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Value" && !LaunchOptions.IsGameRunning.Value)
        {
            if (_viewModel?.LaunchOptions is { } options)
            {
                RefreshResolution(options);
            }
        }
    }

    private void StartConfigSync()
    {
        if (_configSyncTimer is not null) return;
        _configSyncTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _configSyncTimer.Tick += OnConfigSyncTick;
        _configSyncTimer.Start();
    }

    private void StopConfigSync()
    {
        if (_configSyncTimer is null) return;
        _configSyncTimer.Stop();
        _configSyncTimer.Tick -= OnConfigSyncTick;
        _configSyncTimer = null;
    }

    private void OnConfigSyncTick(object? sender, object e)
    {
        if (_viewModel?.LaunchOptions is not { } options) return;
        _isSyncingFromDll = true;
        try { GameProcessFactory.ReadDllConfigIntoOptions(options); }
        finally { _isSyncingFromDll = false; }
    }

    private static void RefreshResolution(LaunchOptions options)
    {
        string? gamePath = options.GamePathEntry?.Value?.Path;
        if (!string.IsNullOrEmpty(gamePath))
        {
            string gameDirectory = System.IO.Path.GetDirectoryName(gamePath) ?? string.Empty;
            options.LoadGameResolutionFromConfig(gameDirectory);
        }
    }

    private void OnResolutionPresetChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is { } options)
        {
            if (options.IsRefreshingFromRegistry)
            {
                return;
            }

            options.ApplyResolutionPreset();
        }
    }

    private void OnWindowedChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            vm.LaunchOptions.SetWindowed(vm.LaunchOptions.IsWindowed.Value);
        }
    }

    private void OnBorderlessChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            vm.LaunchOptions.SetBorderless(vm.LaunchOptions.IsBorderless.Value);
        }
    }

    private void OnFullScreenChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            vm.LaunchOptions.SetFullScreen(vm.LaunchOptions.IsFullScreen.Value);
        }
    }
}
