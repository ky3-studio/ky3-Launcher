//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.Setting;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.Game;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Package;
using kyxsan.Service.Game.Package.Advanced;
using kyxsan.Service.Game.Package.Advanced.Model;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.Notification;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using System.IO;

namespace kyxsan.ViewModel.Game;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Singleton)]
internal sealed partial class GamePackageViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IGamePackageService gamePackageService;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly LaunchGameShared launchGameShared;
    private readonly IServiceProvider serviceProvider;
    private readonly IHoyoPlayService hoyoPlayService;
    private readonly LaunchOptions launchOptions;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial GamePackageViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LocalVersionText), nameof(IsUpdateAvailable))]
    public partial Version? LocalVersion { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(RemoteVersionText), nameof(IsUpdateAvailable))]
    public partial Version? RemoteVersion { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PreVersionText), nameof(IsPredownloadButtonEnabled))]
    public partial Version? PreVersion { get; set; }

    [ObservableProperty]
    public partial string GameSizeText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string SchemeText { get; set; } = string.Empty;

    public string LocalVersionText { get => LocalVersion is null ? "Unknown" : SH.FormatViewModelGamePackageLocalVersion(LocalVersion); }

    public string RemoteVersionText { get => SH.FormatViewModelGamePackageRemoteVersion(RemoteVersion); }

    public string PreVersionText { get => SH.FormatViewModelGamePackagePreVersion(PreVersion); }

    public bool IsUpdateAvailable { get => LocalVersion < RemoteVersion; }

    public bool IsPredownloadButtonEnabled
    {
        get
        {
            if (PreVersion is null)
            {
                return false;
            }

            if (LocalVersion >= PreVersion)
            {
                return false;
            }

            const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(IsPredownloadButtonEnabled)}";
            if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
            {
                return false;
            }

            using (gameFileSystem)
            {
                // IsPredownloadFinished also TryGetGameFileSystem
                return !IsPredownloadFinished;
            }
        }
    }

    public bool IsPredownloadFinished
    {
        get
        {
            const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(IsPredownloadFinished)}";
            if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
            {
                return false;
            }

            ArgumentNullException.ThrowIfNull(gameFileSystem);
            using (gameFileSystem)
            {
                if (!File.Exists(gameFileSystem.PredownloadStatusFilePath))
                {
                    return false;
                }

                if (JsonSerializer.Deserialize<PredownloadStatus>(File.ReadAllText(gameFileSystem.PredownloadStatusFilePath), jsonOptions) is { } predownloadStatus)
                {
                    int fileCount = Directory.GetFiles(gameFileSystem.ChunksDirectory).Length - 1;
                    return predownloadStatus.Finished && fileCount == predownloadStatus.TotalBlocks;
                }
            }

            return false;
        }
    }

    [Command("StartPredownloadCommand")]
    private async Task StartPredownloadAsync()
    {
        if (PreVersion is null || LocalVersion >= PreVersion)
        {
            await contentDialogFactory.CreateForConfirmAsync(
                SH.ViewPageLaunchGamePackageOperationPredownload,
                SH.ViewPageLaunchGamePackagePredownloadUnavailable).ConfigureAwait(false);
            return;
        }

        await ExecutePackageOperationAsync(GamePackageOperationKind.Predownload).ConfigureAwait(false);
    }

    [Command("StartVerifyCommand")]
    private async Task StartVerifyAsync()
    {
        await ExecutePackageOperationAsync(GamePackageOperationKind.Verify).ConfigureAwait(false);
    }

    [Command("StartUpdateCommand")]
    private async Task StartUpdateAsync()
    {
        await ExecutePackageOperationAsync(GamePackageOperationKind.Update).ConfigureAwait(false);
    }

    private async Task ExecutePackageOperationAsync(GamePackageOperationKind kind)
    {
        if (launchGameShared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } launchScheme)
        {
            return;
        }

        if (await GetCurrentGameBranchAsync(launchScheme).ConfigureAwait(false) is not { } branch)
        {
            return;
        }

        string lockTrace = $"{nameof(GamePackageViewModel)}.{kind}";
        if (launchOptions.TryGetGameFileSystem(lockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);

        if (await GetSophonDecodedBuildsAsync(kind, branch, gameFileSystem).ConfigureAwait(false) is not { } builds)
        {
            return;
        }

        GamePackageOperationContext context = new(serviceProvider, kind, gameFileSystem)
        {
            LocalBuild = builds.LocalBuild,
            RemoteBuild = builds.RemoteBuild,
            PatchBuild = builds.PatchBuild,
        };

        await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false);
    }

    public async ValueTask ReloadAsync()
    {
        bool result = await LoadOverrideAsync(CancellationToken).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        IsInitialized = result;
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (launchGameShared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } launchScheme)
        {
            return false;
        }

        string schemeName = (launchScheme.Channel, launchScheme.IsOversea) switch
        {
            (Model.Intrinsic.ChannelType.Bili, false) => SH.ModelBindingLaunchGameLaunchSchemeBilibili,
            (_, false) => SH.ModelBindingLaunchGameLaunchSchemeChinese,
            (_, true) => SH.ModelBindingLaunchGameLaunchSchemeOversea,
        };

        if (await GetCurrentGameBranchAsync(launchScheme).ConfigureAwait(false) is not { } branch)
        {
            return false;
        }

        await taskContext.SwitchToMainThreadAsync();

        SchemeText = string.Format(SH.ViewModelGamePackageSchemeText, schemeName);

        (BranchWrapper remote, BranchWrapper? pre) = LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
            ? (branch.PreDownload ?? branch.Main, default)
            : (branch.Main, branch.PreDownload);

        (RemoteVersion, PreVersion) = (new(remote.Tag), pre is { Tag: { } preTag } ? new(preTag) : default);

        const string LockTrace = $"{nameof(GamePackageViewModel)}.{nameof(LoadOverrideAsync)}";
        if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            return false;
        }

        ArgumentNullException.ThrowIfNull(gameFileSystem);
        using (gameFileSystem)
        {
            if (gameFileSystem.TryGetGameVersion(out string? localVersion))
            {
                _ = Version.TryParse(localVersion, out Version? version);
                LocalVersion = version;
            }

            if (!IsUpdateAvailable && PreVersion is null && File.Exists(gameFileSystem.PredownloadStatusFilePath))
            {
                File.Delete(gameFileSystem.PredownloadStatusFilePath);
            }

            string gameDir = gameFileSystem.GameDirectory;
            _ = Task.Run(
                () =>
                {
                    try
                    {
                        long totalBytes = 0;
                        foreach (string file in Directory.EnumerateFiles(gameDir, "*", SearchOption.AllDirectories))
                        {
                            try { totalBytes += new FileInfo(file).Length; }
                            catch { }
                        }

                        double gb = totalBytes / (1024.0 * 1024.0 * 1024.0);
                        string sizeText = string.Format(SH.ViewModelGamePackageSizeText, $"{gb:F1}");
                        taskContext.InvokeOnMainThread(() => GameSizeText = sizeText);
                    }
                    catch { }
                },
                token);
        }

        return true;
    }

    private async ValueTask<SophonDecodedBuilds?> GetSophonDecodedBuildsAsync(GamePackageOperationKind operationKind, GameBranch branch, IGameFileSystem gameFileSystem)
    {
        ArgumentNullException.ThrowIfNull(LocalVersion);

        SophonDecodedBuild? localBuild;
        SophonDecodedBuild? remoteBuild;
        SophonDecodedPatchBuild? patchBuild;

        try
        {
            BranchWrapper localBranch = operationKind is GamePackageOperationKind.Verify && LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
                ? branch.PreDownload?.GetTaggedCopy(LocalVersion.ToString()) ?? branch.Main.GetTaggedCopy(LocalVersion.ToString())
                : branch.Main.GetTaggedCopy(LocalVersion.ToString());
            localBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, localBranch).ConfigureAwait(false);

            BranchWrapper? remoteBranch = operationKind is GamePackageOperationKind.Update && LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false)
                ? branch.PreDownload ?? branch.Main
                : operationKind is GamePackageOperationKind.Predownload ? branch.PreDownload : branch.Main;
            remoteBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);

            patchBuild = await gamePackageService.DecodeDiffManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);

            ArgumentNullException.ThrowIfNull(localBuild);
            ArgumentNullException.ThrowIfNull(remoteBuild);
        }
        catch (Exception ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
            return default;
        }

        return new()
        {
            LocalBuild = localBuild,
            RemoteBuild = remoteBuild,
            PatchBuild = patchBuild,
        };
    }

    private async ValueTask<GameBranch?> GetCurrentGameBranchAsync(LaunchScheme launchScheme)
    {
        if (await hoyoPlayService.TryGetBranchesAsync(launchScheme).ConfigureAwait(false) is not (true, { } branchesWrapper))
        {
            return default;
        }

        if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } branch)
        {
            messenger.Send(InfoBarMessage.Error(SH.ViewModelGamePackageGetGameBranchFailed, SH.FormatViewModelGamePackageLocalLaunchScheme(launchScheme.DisplayName)));
            return default;
        }

        return branch;
    }

    private async ValueTask<GameChannelSDK?> GetCurrentGameChannelSDKAsync(LaunchScheme launchScheme)
    {
        if (await hoyoPlayService.TryGetChannelSDKsAsync(launchScheme).ConfigureAwait(false) is not (true, { } channelSDKsWrapper))
        {
            return default;
        }

        // Channel sdk can be null
        return channelSDKsWrapper.GameChannelSDKs.FirstOrDefault(sdk => sdk.Game.Id == launchScheme.GameId);
    }

    private sealed class SophonDecodedBuilds
    {
        public required SophonDecodedBuild? LocalBuild { get; init; }

        public required SophonDecodedBuild? RemoteBuild { get; init; }

        public required SophonDecodedPatchBuild? PatchBuild { get; init; }
    }
}