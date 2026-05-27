//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.Input;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.Logging;
using kyxsan.Core.Shell;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Process;
using kyxsan.Service.Game;
using kyxsan.Service.Game.AdvancedStart;
using kyxsan.Service.Game.AdvancedStart.Model;
using kyxsan.Service.Game.Configuration;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Launching;
using kyxsan.Service.Game.Launching.Context;
using kyxsan.Service.Game.Launching.Invoker;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.Navigation;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.UI.Xaml.View.Page;
using kyxsan.ViewModel.User;
using System.IO;

namespace kyxsan.ViewModel.Game;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class LaunchGameShared
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly INavigationService navigationService;
    private readonly IServiceProvider serviceProvider;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;
    private readonly IGameService gameService;
    private readonly IMessenger messenger;

    private bool resuming;

    [GeneratedConstructor]
    public partial LaunchGameShared(IServiceProvider serviceProvoder);

    public LaunchScheme? GetCurrentLaunchSchemeFromConfigurationFile(bool showInfo = true)
    {
        ChannelOptions options = gameService.GetChannelOptions();

        if (options.ErrorKind is ChannelOptionsErrorKind.None)
        {
            try
            {
                return KnownLaunchSchemes.Values.Single(scheme => scheme.Equals(options));
            }
            catch (InvalidOperationException)
            {
                kyxsanException.Throw(string.Format(SH.ViewModelLaunchGameUnsupportedChannelOptions, options));
            }
        }

        if (!showInfo)
        {
            return default;
        }

        InfoBarMessage? message = options.ErrorKind switch
        {
            ChannelOptionsErrorKind.ConfigurationFileNotFound => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.FormatViewModelLaunchGameConfigurationFileNotFound(options.FilePath),
                SH.ViewModelLaunchGameFixConfigurationFileButtonText,
                HandleConfigurationFileNotFoundCommand),
            ChannelOptionsErrorKind.GamePathNullOrEmpty => default,
            ChannelOptionsErrorKind.DeviceNotFound => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.ViewModelLaunchGameDeviceNotFound),
            ChannelOptionsErrorKind.GameContentCorrupted => InfoBarMessage.Warning(
                SH.FormatViewModelLaunchGameConfigurationFailed(options.ErrorKind),
                SH.FormatViewModelLaunchGameContentCorrupted(options.FilePath)),
            ChannelOptionsErrorKind.GamePathLocked => InfoBarMessage.Error(
                SH.ViewModelGameConfigurationCreateFailedGamePathLocked,
                options.FilePath ?? string.Empty),
            _ => default,
        };

        if (message is not null)
        {
            messenger.Send(message);
        }

        return default;
    }

    public async ValueTask DefaultLaunchExecutionAsync(IViewModelSupportLaunchExecution viewModel, UserAndUid? userAndUid)
    {
        // The game process can exist longer than the view model
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DefaultLaunchExecutionInvoker invoker = new();
            try
            {
                LaunchExecutionInvocationContext context = new()
                {
                    ViewModel = viewModel,
                    ServiceProvider = scope.ServiceProvider,
                    LaunchOptions = scope.ServiceProvider.GetRequiredService<LaunchOptions>(),
                    Identity = GameIdentity.Create(userAndUid, viewModel.GameAccount),
                };

                await invoker.InvokeAsync(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                scope.ServiceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(SH.ViewModelLaunchGameErrorTitle, ex.Message));
            }
        }
    }

    public async ValueTask ResumeLaunchExecutionAsync(IViewModelSupportLaunchExecution viewModel)
    {
        if (Interlocked.Exchange(ref resuming, true))
        {
            return;
        }

        try
        {
            if (!await GameLifeCycle.IsGameRunningAsync(taskContext).ConfigureAwait(false))
            {
                return;
            }

            if (AbstractLaunchExecutionInvoker.Invoking())
            {
                return;
            }

            if (GetCurrentLaunchSchemeFromConfigurationFile(false) is null)
            {
                return;
            }

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                try
                {
                    LaunchExecutionInvocationContext context = new()
                    {
                        ViewModel = viewModel,
                        ServiceProvider = scope.ServiceProvider,
                        LaunchOptions = launchOptions,
                        Identity = GameIdentity.Create(),
                    };

                    await new ResumeLaunchExecutionInvoker().InvokeAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    messenger.Send(InfoBarMessage.Error(SH.ViewModelResumeGameErrorTitle, ex.Message));
                }
            }
        }
        finally
        {
            Volatile.Write(ref resuming, false);
        }
    }

    public async ValueTask<bool> ConvertLaunchExecutionAsync(IViewModelSupportLaunchExecution viewModel)
    {
        ConvertOnlyLaunchExecutionInvoker invoker = new();
        try
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                LaunchExecutionInvocationContext context = new()
                {
                    ViewModel = viewModel,
                    ServiceProvider = scope.ServiceProvider,
                    LaunchOptions = launchOptions,
                    Identity = GameIdentity.Create(),
                };

                await invoker.InvokeAsync(context).ConfigureAwait(false);
            }

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            if (Environment.IsPrivilegedProcess)
            {
                messenger.Send(InfoBarMessage.Error(SH.ViewModelServerConvertTitle, SH.ViewModelServerConvertAccessDeniedAdmin));
            }
            else
            {
                messenger.Send(InfoBarMessage.Warning(
                    SH.ViewModelServerConvertTitle,
                    SH.ViewModelServerConvertAccessDenied,
                    SH.ViewModelServerConvertRestartAsAdmin,
                    new RelayCommand(static () => NativeMethods.RestartAsAdministrator())));
            }

            return false;
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelServerConvertTitle, ex.Message));
            return false;
        }
    }

    public async Task LaunchAdvancedDelayedAsync(CancellationToken token = default)
    {
        List<AdvancedStartDelayedProgramEntry> snapshot;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            AdvancedStartDelayedProgramStore store = scope.ServiceProvider.GetRequiredService<AdvancedStartDelayedProgramStore>();
            snapshot = store.Load().ToList();
        }

        List<Task> tasks = new(snapshot.Count);
        foreach (AdvancedStartDelayedProgramEntry entry in snapshot)
        {
            tasks.Add(Task.Run(async () =>
            {
                token.ThrowIfCancellationRequested();

                int delaySeconds = Math.Max(0, entry.DelaySeconds);
                if (delaySeconds > 0)
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(delaySeconds), token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }

                if (string.IsNullOrWhiteSpace(entry.Path) || !File.Exists(entry.Path))
                {
                    messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameAdvancedStartProgramNotExists, entry.Path));
                    return;
                }

                try
                {
                    ProcessFactory.StartUsingShellExecute(string.Empty, entry.Path);
                }
                catch (Exception ex)
                {
                    messenger.Send(InfoBarMessage.Error(ex));
                }
            }, token));
        }

        try
        {
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Swallow cancellation; caller expects cancellation to stop launching.
        }
    }

    [Command("HandleConfigurationFileNotFoundCommand")]
    private async Task HandleConfigurationFileNotFoundAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Generate config file", "LaunchGameShared.Command"));

        const string LockTrace = $"{nameof(LaunchGameShared)}.{nameof(HandleConfigurationFileNotFoundAsync)}";
        GameFileSystemErrorKind errorKind = launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem);
        switch (errorKind)
        {
            case GameFileSystemErrorKind.GamePathLocked:
                messenger.Send(InfoBarMessage.Warning(SH.ViewModelGameConfigurationCreateFailed, SH.ViewModelGameConfigurationCreateFailedGamePathLocked));
                return;
            case GameFileSystemErrorKind.GamePathNullOrEmpty:
                messenger.Send(InfoBarMessage.Warning(SH.ViewModelGameConfigurationCreateFailed, SH.ViewModelGameConfigurationCreateFailedGamePathNullOrEmpty));
                return;
            default:
                ArgumentNullException.ThrowIfNull(gameFileSystem);
                break;
        }

        using (gameFileSystem)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                LaunchGameConfigurationFixDialog dialog = await contentDialogFactory
                    .CreateInstanceAsync<LaunchGameConfigurationFixDialog>(scope.ServiceProvider)
                    .ConfigureAwait(false);

                bool isOversea = gameFileSystem.IsExecutableOversea;

                await taskContext.SwitchToMainThreadAsync();

                dialog.KnownSchemes = KnownLaunchSchemes.Values.Where(scheme => scheme.IsOversea == isOversea);
                dialog.SelectedScheme = dialog.KnownSchemes.First(scheme => scheme.IsNotCompatOnly);

                if (await dialog.GetLaunchSchemeAsync().ConfigureAwait(false) is not (true, { } launchScheme))
                {
                    return;
                }

                _ = GameConfiguration.Patch(launchScheme, gameFileSystem.ScriptVersionFilePath, gameFileSystem.GameConfigurationFilePath)
                    ? messenger.Send(InfoBarMessage.Success(SH.ViewModelLaunchGameFixConfigurationFileSucceed))
                    : messenger.Send(InfoBarMessage.Error(SH.ViewModelLaunchGameFixConfigurationFileFailed));
            }
        }
    }

    [Command("HandleGamePathNullOrEmptyCommand")]
    private void HandleGamePathNullOrEmpty()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Navigate to LaunchGamePage", "LaunchGameShared.Command"));
        navigationService.Navigate<LaunchGamePage>(NavigationExtraData.Default, true);
    }
}