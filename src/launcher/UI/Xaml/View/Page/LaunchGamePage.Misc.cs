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
using Microsoft.UI.Xaml.Input;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Game.Account;
using Launcher.ViewModel.Game;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class LaunchGamePage
{
    private bool hdrManualToggleInitializing;

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
}
