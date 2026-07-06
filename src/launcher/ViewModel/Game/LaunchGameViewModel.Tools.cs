//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.IO;
using Launcher.Core.Logging;
using Launcher.Factory.ContentDialog;
using Launcher.Factory.Picker;
using Launcher.Service.ThirdPartyTool;
using Launcher.UI.Xaml.View.Dialog;
using Launcher.Web.ThirdPartyTool;
using System.Collections.Immutable;

namespace Launcher.ViewModel.Game;

internal sealed partial class LaunchGameViewModel
{
    private async Task InitializeThirdPartyToolsInBackgroundAsync(CancellationToken token)
    {
        try
        {
            await Task.Yield();

            if (token.IsCancellationRequested || IsViewUnloaded.Value)
            {
                return;
            }

            ImmutableArray<ToolInfo> tools = await InitializeThirdPartyToolsAsync(token).ConfigureAwait(false);

            if (token.IsCancellationRequested || IsViewUnloaded.Value)
            {
                return;
            }

            await taskContext.SwitchToMainThreadAsync();
            if (!token.IsCancellationRequested && !IsViewUnloaded.Value)
            {
                thirdPartyToolsField.Value = tools;
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb($"Failed to initialize third party tools: {ex.Message}", category: "ThirdPartyTool");
            SentrySdk.CaptureException(ex);
        }
    }

    private async ValueTask<ImmutableArray<ToolInfo>> InitializeThirdPartyToolsAsync(CancellationToken token)
    {
        try
        {
            SentrySdk.AddBreadcrumb("Starting to initialize third party tools", category: "ThirdPartyTool");
            IThirdPartyToolService thirdPartyToolService = serviceProvider.GetRequiredService<IThirdPartyToolService>();
            SentrySdk.AddBreadcrumb("Got IThirdPartyToolService instance", category: "ThirdPartyTool");

            token.ThrowIfCancellationRequested();
            ImmutableArray<ToolInfo> tools = await thirdPartyToolService.GetToolsAsync(token).ConfigureAwait(false);
            token.ThrowIfCancellationRequested();

            SentrySdk.AddBreadcrumb($"Got {tools.Length} tools from service", category: "ThirdPartyTool");
            return tools;
        }
        catch (OperationCanceledException)
        {
            return ImmutableArray<ToolInfo>.Empty;
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb($"Failed to initialize third party tools: {ex.Message}", category: "ThirdPartyTool");
            SentrySdk.CaptureException(ex);
            return ImmutableArray<ToolInfo>.Empty;
        }
    }

    [Command("ShowThirdPartyToolDialogCommand")]
    private async Task ShowThirdPartyToolDialogAsync(ToolInfo? tool)
    {
        if (tool is null)
        {
            return;
        }

        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Show third party tool dialog", "LaunchGameViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ThirdPartyToolDialog dialog = await scope.ServiceProvider
                .GetRequiredService<IContentDialogFactory>()
                .CreateInstanceAsync<ThirdPartyToolDialog>(scope.ServiceProvider, tool);

            await contentDialogFactory.EnqueueAndShowAsync(dialog).ShowTask;
        }
    }

    [Command("PickBgiPathCommand")]
    private async Task PickBgiPathAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.BgiPath.Value = file;
    }

    [Command("PickAttachProgramCommand")]
    private async Task PickAttachProgramAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.AttachProgramPath.Value = file;
    }

    [Command("PickAttachProgram2Command")]
    private async Task PickAttachProgram2Async()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.AttachProgram2Path.Value = file;
    }

    [Command("PickAttachProgram3Command")]
    private async Task PickAttachProgram3Async()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "program", "*.exe");
        if (!ok)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        LaunchOptions.AttachProgram3Path.Value = file;
    }

    [Command("AddCustomDllCommand")]
    private async Task AddCustomDllAsync()
    {
        await taskContext.SwitchToBackgroundAsync();
        (bool ok, ValueFile file) = fileSystemPickerInteraction.PickFile("Picker", "dll", "*.dll");
        if (!ok)
        {
            return;
        }

        string path = file;

        await taskContext.SwitchToMainThreadAsync();
        ImmutableDictionary<string, bool> current = LaunchOptions.CustomDllConfigs.Value;
        if (!current.ContainsKey(path))
        {
            LaunchOptions.CustomDllConfigs.Value = current.Add(path, true);
        }
    }

    [Command("RemoveCustomDllCommand")]
    private void RemoveCustomDll(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ImmutableDictionary<string, bool> current = LaunchOptions.CustomDllConfigs.Value;
        LaunchOptions.CustomDllConfigs.Value = current.Remove(path);
    }

    [Command("ToggleCustomDllCommand")]
    private void ToggleCustomDll(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        ImmutableDictionary<string, bool> current = LaunchOptions.CustomDllConfigs.Value;
        if (current.TryGetValue(path, out bool enabled))
        {
            LaunchOptions.CustomDllConfigs.Value = current.SetItem(path, !enabled);
        }
    }
}
