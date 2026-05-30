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
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.Game;
using kyxsan.Service.Game.Launching;
using kyxsan.Service.Game.Account;
using kyxsan.Service.RemoteConfig;
using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.Game;

namespace kyxsan.UI.Xaml.View.Page;

[SuppressMessage("", "CA1001")]
internal sealed partial class LaunchGamePage : ScopedPage
{
    private LaunchGameViewModel? _viewModel;
    private CancellationTokenSource? _refreshCts;
    private DispatcherTimer? _configSyncTimer;
    private bool _isSyncingFromDll;
    private bool _injectionToggleChanging = true;

    public LaunchGamePage()
    {
        InitializeComponent();
        Unloaded += OnPageUnloaded;
        Loaded += OnPageLoaded;
        InjectionOptionsConfigService.Changed += OnInjectionConfigChanged;
    }

    private void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        hdrManualToggleInitializing = true;
        HdrManualToggle.IsOn = WindowsHDRControl.IsHDROn();
        hdrManualToggleInitializing = false;

        DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
        {
            _injectionToggleChanging = false;
        });

        _ = Task.CompletedTask; // Start background polling for injection option visibility
        InjectionOptionsConfigService.StartPolling();
    }

    private void OnInjectionConfigChanged(HashSet<string> disabled)
    {
        DispatcherQueue?.TryEnqueue(() => ApplyVisibility(disabled));
    }

    private void ApplyVisibility(HashSet<string> disabled)
    {
        (string key, UIElement[] elements)[] mapping =
        [
            ("fov", [Opt_fov, Opt_fov_val]),
            ("disable_fog", [Opt_disable_fog]),
            ("portable_craft", [Opt_portable_craft]),
            ("fps", [Opt_fps, Opt_fps_val]),
            ("disable_char_fade", [Opt_disable_char_fade]),
            ("disable_vsync", [Opt_disable_vsync]),
            ("hide_quest_banner", [Opt_hide_quest_banner]),
            ("hide_menu_uid", [Opt_hide_menu_uid]),
            ("redirect_combine", [Opt_redirect_combine, Opt_redirect_combine_val]),
            ("remove_team_progress", [Opt_remove_team_progress]),
            ("disable_event_camera", [Opt_disable_event_camera]),
            ("disable_damage_text", [Opt_disable_damage_text]),
            ("touch_screen", [Opt_touch_screen]),
            ("redirect_dispatch", [Opt_redirect_dispatch, Opt_redirect_dispatch_val]),
            ("enable_cooking", [Opt_enable_cooking, Opt_enable_cooking_val]),
            ("enable_forge", [Opt_enable_forge, Opt_enable_forge_val]),
            ("hide_uid", [Opt_hide_uid]),
        ];

        foreach ((string key, UIElement[] elements) in mapping)
        {
            Visibility vis = disabled.Contains(key) ? Visibility.Collapsed : Visibility.Visible;
            foreach (UIElement el in elements)
            {
                el.Visibility = vis;
            }
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
        }
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (_viewModel?.LaunchOptions is { } options)
        {
            RefreshResolution(options);
        }
        ApplyVisibility(InjectionOptionsConfigService.Current);
    }

    private void OnPageUnloaded(object sender, RoutedEventArgs e)
    {
        StopConfigSync();
        _refreshCts?.Cancel();
        _refreshCts?.Dispose();
        _refreshCts = null;

        if (_viewModel?.LaunchOptions is { } options)
        {
            LaunchOptions.IsGameRunning.PropertyChanged -= OnIsGameRunningChanged;
        }
    }

    private void StartAutoRefresh()
    {
        _refreshCts = new CancellationTokenSource();
        CancellationToken token = _refreshCts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(2000, token).ConfigureAwait(false);
                if (token.IsCancellationRequested)
                    break;

                try
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (_viewModel?.LaunchOptions is { } options)
                        {
                            options.RefreshFromRegistry();
                        }
                    }).AsTask().ConfigureAwait(false);
                }
                catch { }
            }
        }, token);
    }

    private void OnIsGameRunningChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "Value")
        {
            if (LaunchOptions.IsGameRunning.Value)
            {
                StartConfigSync();
            }
            else
            {
                StopConfigSync();
                if (_viewModel?.LaunchOptions is { } options)
                {
                    RefreshResolution(options);
                }
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

    private void OnGridTapped(object sender, TappedRoutedEventArgs e)
    {
        if (e.OriginalSource is Microsoft.UI.Xaml.Controls.Grid)
        {
            Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        }
    }

    private async void OnServerInfoClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel?.TargetSchemeFilteredGameAccountsView?.Scheme is { } scheme)
        {
            string title = scheme.DisplayName;
            string description = scheme.Description;

            ContentDialog dialog = new()
            {
                Title = title,
                Content = new TextBlock
                {
                    Text = string.IsNullOrEmpty(description) ? SH.ViewPageLaunchGameServerInfoNoDescription : description,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 8, 0, 0)
                },
                CloseButtonText = SH.ViewCommonConfirmButton,
                XamlRoot = this.XamlRoot
            };
            await Ioc.Default.GetRequiredService<IContentDialogFactory>().EnqueueAndShowAsync(dialog).ShowTask;
        }
    }

    private void OnIslandOptionChanged(object sender, RoutedEventArgs e)
    {
        if (_isSyncingFromDll) return;
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            GameProcessFactory.UpdateDllConfigForHotSwitch(vm.LaunchOptions);
        }
    }

    private void OnFpsToggled(object sender, RoutedEventArgs e)
    {
        if (_isSyncingFromDll) return;
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            GameProcessFactory.UpdateDllConfigForHotSwitch(vm.LaunchOptions);
        }
    }

    private void OnCustomDllToggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggle && toggle.Tag is string path)
        {
            if (DataContext is LaunchGameViewModel vm)
            {
                vm.ToggleCustomDllCommand.Execute(path);
            }
        }
    }

    private void OnResolutionPresetChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            vm.LaunchOptions.ApplyResolutionPreset();
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

    private void OnIslandValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (_isSyncingFromDll) return;
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            GameProcessFactory.UpdateDllConfigForHotSwitch(vm.LaunchOptions);
        }
    }

    private void OnSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        if (_isSyncingFromDll) return;
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            GameProcessFactory.UpdateDllConfigForHotSwitch(vm.LaunchOptions);
        }
    }

    private void OnValueTextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (_isSyncingFromDll) return;
        if (DataContext is LaunchGameViewModel vm && vm.LaunchOptions is not null)
        {
            GameProcessFactory.UpdateDllConfigForHotSwitch(vm.LaunchOptions);
        }
    }

    private void OnSelectBgiPathClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.PickBgiPathCommand.Execute(null);
        }
    }

    private void OnSelectAttachProgram1Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.PickAttachProgramCommand.Execute(null);
        }
    }

    private void OnSelectAttachProgram2Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.PickAttachProgram2Command.Execute(null);
        }
    }

    private void OnSelectAttachProgram3Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.PickAttachProgram3Command.Execute(null);
        }
    }

    private void OnClearBgiPathClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.LaunchOptions.BgiPath.Value = string.Empty;
        }
    }

    private void OnClearAttachProgram1Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.LaunchOptions.AttachProgramPath.Value = string.Empty;
        }
    }

    private void OnClearAttachProgram2Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.LaunchOptions.AttachProgram2Path.Value = string.Empty;
        }
    }

    private void OnClearAttachProgram3Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is LaunchGameViewModel vm)
        {
            vm.LaunchOptions.AttachProgram3Path.Value = string.Empty;
        }
    }

    private bool hdrManualToggleInitializing;

    private static string InjectionConfirmText => SH.ViewPageLaunchGameInjectionConfirmText;

    private async void OnInjectionToggled(object sender, RoutedEventArgs e)
    {
        if (_injectionToggleChanging)
        {
            return;
        }

        ToggleSwitch toggle = (ToggleSwitch)sender;

        if (!toggle.IsOn)
        {
            if (DataContext is LaunchGameViewModel vm)
            {
                ResetIslandOptions(vm.LaunchOptions);
            }

            return;
        }

        _injectionToggleChanging = true;
        toggle.IsOn = false;
        _injectionToggleChanging = false;

        if (DataContext is LaunchGameViewModel { LaunchOptions.IsIslandRiskAccepted.Value: true })
        {
            _injectionToggleChanging = true;
            toggle.IsOn = true;
            _injectionToggleChanging = false;
            return;
        }

        TextBox inputBox = new()
        {
            PlaceholderText = SH.ViewPageLaunchGameInjectionInputPlaceholder,
            AcceptsReturn = false,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        inputBox.Paste += (_, args) => args.Handled = true;

        StackPanel content = new() { Spacing = 16, MinWidth = 480 };
        content.Children.Add(new TextBlock
        {
            Text = SH.ViewPageLaunchGameInjectionDisclaimerTitle,
            Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
        });
        content.Children.Add(new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Text = SH.ViewPageLaunchGameInjectionDisclaimerContent,
        });
        content.Children.Add(new TextBlock
        {
            Text = InjectionConfirmText,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            IsTextSelectionEnabled = false,
            Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemFillColorCriticalBrush"],
        });
        content.Children.Add(inputBox);

        ContentDialog dialog = new()
        {
            Title = SH.ViewPageLaunchGameInjectionDialogTitle,
            Content = content,
            PrimaryButtonText = SH.ViewPageLaunchGameInjectionConfirmButton,
            CloseButtonText = SH.ViewCommonCancelButton,
            DefaultButton = ContentDialogButton.Close,
            XamlRoot = XamlRoot,
        };

        IContentDialogFactory dialogFactory = Ioc.Default.GetRequiredService<IContentDialogFactory>();

        try
        {
            ContentDialogResult result = await dialogFactory.EnqueueAndShowAsync(dialog).ShowTask;

            if (result != ContentDialogResult.Primary)
            {
                return;
            }

            if (inputBox.Text.Trim() != InjectionConfirmText)
            {
                ContentDialog errorDialog = new()
                {
                    Title = SH.ViewPageLaunchGameInjectionErrorTitle,
                    Content = SH.ViewPageLaunchGameInjectionErrorContent,
                    CloseButtonText = SH.ViewCommonConfirmButton,
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = XamlRoot,
                };
                await dialogFactory.EnqueueAndShowAsync(errorDialog).ShowTask;
                return;
            }

            _injectionToggleChanging = true;
            toggle.IsOn = true;
            _injectionToggleChanging = false;

            if (DataContext is LaunchGameViewModel accepted)
            {
                accepted.LaunchOptions.IsIslandRiskAccepted.Value = true;
            }
        }
        catch
        {
        }
    }

    private static void ResetIslandOptions(LaunchOptions options)
    {
        options.IsSetFieldOfViewEnabled.Value = false;
        options.TargetFov.Value = 45f;
        options.FixLowFovScene.Value = false;
        options.DisableFog.Value = false;
        options.RemoveOpenTeamProgress.Value = false;
        options.HideQuestBanner.Value = false;
        options.DisableEventCameraMove.Value = false;
        options.DisableShowDamageText.Value = false;
        options.UsingTouchScreen.Value = false;
        options.EnablePortableCraftingBench.Value = false;
        options.RedirectCombineEntry.Value = false;
        options.DisableCharFade.Value = false;
        options.HideUID.Value = false;
        options.HideMenuUID.Value = false;
        options.DisableVSync.Value = false;
        options.EnableFps.Value = false;
        options.TargetFps.Value = 60;
    }

    private async void OnHdrManualToggled(object sender, RoutedEventArgs e)
    {
        if (hdrManualToggleInitializing)
            return;

        ToggleSwitch toggle = (ToggleSwitch)sender;
        bool wantOn = toggle.IsOn;

        if (wantOn && !WindowsHDRControl.IsHDRSupported())
        {
            hdrManualToggleInitializing = true;
            toggle.IsOn = false;
            hdrManualToggleInitializing = false;

            ContentDialog dialog = new()
            {
                Title = SH.ViewPageLaunchGameHdrUnavailableTitle,
                Content = SH.ViewPageLaunchGameHdrUnavailableContent,
                CloseButtonText = SH.ViewCommonConfirmButton,
                XamlRoot = this.XamlRoot,
            };
            await Ioc.Default.GetRequiredService<IContentDialogFactory>().EnqueueAndShowAsync(dialog).ShowTask;
            return;
        }

        WindowsHDRControl.SetHDR(wantOn);
    }
}
