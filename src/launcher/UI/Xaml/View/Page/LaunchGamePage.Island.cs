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
using Microsoft.UI.Xaml.Controls.Primitives;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Game;
using Launcher.Service.Game.Launching;
using Launcher.ViewModel.Game;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LaunchGamePage
{
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

        ScrollViewer scrollWrapper = new()
        {
            Content = content,
            MaxHeight = 480,
            VerticalScrollBarVisibility = Microsoft.UI.Xaml.Controls.ScrollBarVisibility.Auto,
        };

        ContentDialog dialog = new()
        {
            Title = SH.ViewPageLaunchGameInjectionDialogTitle,
            Content = scrollWrapper,
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
        options.EnableFreeCam.Value = false;
        options.FreeCamKey.Value = 0x75;
        options.FreeCamModifier.Value = 0;
        options.EnableFreeCamLock.Value = true;
        options.FreeCamLockKey.Value = 0x2E;
        options.FreeCamLockModifier.Value = 0;
        options.FreeCamMoveSpeed.Value = 0.35f;
        options.FreeCamSprintMult.Value = 3.0f;
        options.FreeCamMouseSensitivity.Value = 0.12f;
        options.FreeCamPitchLimit.Value = 89f;
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
}
