//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core.DataTransfer;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.Graphics;
using kyxsan.Core.IO;
using kyxsan.Core.LifeCycle;
using kyxsan.Core.Logging;
using kyxsan.Core.Setting;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Service.Game;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Package.Advanced;
using kyxsan.Service.Game.Package.Advanced.Model;
using kyxsan.Service.Game.Scheme;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Notification;
using kyxsan.UI.Windowing;
using kyxsan.UI.Xaml.View.Window;
using kyxsan.ViewModel.Game;
using kyxsan.ViewModel.Guide;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;
using kyxsan.Web.Hoyolab.Takumi.Downloader.Proto;
using kyxsan.Web.kyxsan.kyxsanAsAService;
using kyxsan.Web.kyxsan.Redeem;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Response;
using System.IO;
using System.Text.RegularExpressions;

// ReSharper disable LocalizableElement
namespace kyxsan.ViewModel;
[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class TestViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IClipboardProvider clipboardProvider;
    private readonly kyxsanUserOptions kyxsanUserOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<TestViewModel> logger;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial TestViewModel(IServiceProvider serviceProvider);

    public UploadAnnouncement Announcement { get; set => SetProperty(ref field, value); } = new();

    public ExtractOptions ExtractExeOptions { get; } = new();

    public int CdnCompensationDays { get; set; } = 15;

    public DesignationOptions CdnDesignationOptions { get; } = new();

    public RedeemCodeGenerateOptions RedeemCodeGenerateOption { get; set => SetProperty(ref field, value); } = new();

    public bool OverrideElevationRequirement
    {
        get => LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false);
        set => LocalSetting.Set(SettingKeys.OverrideElevationRequirement, value);
    }

    public bool OverrideUpdateVersionComparison
    {
        get => LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false);
        set => LocalSetting.Set(SettingKeys.OverrideUpdateVersionComparison, value);
    }

    public bool OverridePackageConvertDirectoryPermissionsRequirement
    {
        get => LocalSetting.Get(SettingKeys.LaunchOverridePackageConvertDirectoryPermissions, false);
        set => LocalSetting.Set(SettingKeys.LaunchOverridePackageConvertDirectoryPermissions, value);
    }

    public bool OverrideHardDriveType
    {
        get => LocalSetting.Get(SettingKeys.OverridePhysicalDriverType, false);
        set => LocalSetting.Set(SettingKeys.OverridePhysicalDriverType, value);
    }

    public bool OverrideHardDriveTypeIsSolidState
    {
        get => LocalSetting.Get(SettingKeys.PhysicalDriverIsAlwaysSolidState, false);
        set => LocalSetting.Set(SettingKeys.PhysicalDriverIsAlwaysSolidState, value);
    }

    public bool AlwaysIsFirstRunAfterUpdate
    {
        get => LocalSetting.Get(SettingKeys.AlwaysIsFirstRunAfterUpdate, false);
        set => LocalSetting.Set(SettingKeys.AlwaysIsFirstRunAfterUpdate, value);
    }

    public bool TreatPredownloadAsMain
    {
        get => LocalSetting.Get(SettingKeys.TreatPredownloadAsMain, false);
        set => LocalSetting.Set(SettingKeys.TreatPredownloadAsMain, value);
    }

    public bool EnableBetaGameInstall
    {
        get => LocalSetting.Get(SettingKeys.EnableBetaGameInstall, false);
        set => LocalSetting.Set(SettingKeys.EnableBetaGameInstall, value);
    }

    [GeneratedRegex(@"AssetBundles.*\.blk$", RegexOptions.IgnoreCase)]
    private static partial Regex AssetBundlesBlockRegex { get; }

    [GeneratedRegex(@"^(Yuanshen|GenshinImpact)\.exe$", RegexOptions.IgnoreCase)]
    private static partial Regex GameExecutableFileRegex { get; }

    [Command("ResetGuideStateCommand")]
    private static void ResetGuideState()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Reset guide state", "TestViewModel.Command"));
        UnsafeLocalSetting.Set(SettingKeys.GuideState, GuideState.Language);
    }

    [Command("ExceptionCommand")]
    private static void ThrowTestException()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Throw test exception", "TestViewModel.Command"));

        InvalidOperationException inner = new()
        {
            HResult = 0x12345678,
        };
        kyxsanException.Throw("Test Exception", inner);
    }

    [Command("FileOperationRenameCommand")]
    private static void FileOperationRename()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test file operation rename", "TestViewModel.Command"));

        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string source = Path.Combine(desktop, "TestFolder");
        DirectoryOperation.UnsafeRename(source, "TestFolder1");
    }

    [Command("ResetMainWindowSizeCommand")]
    private void ResetMainWindowSize()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Resize MainWindow", "TestViewModel.Command"));

        if (currentXamlWindowReference.Window is MainWindow mainWindow)
        {
            double scale = mainWindow.RasterizationScale;
            mainWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32(1372, 772).Scale(scale));
        }
    }

    [Command("UploadAnnouncementCommand")]
    private async Task UploadAnnouncementAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload kyxsan announcement", "TestViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await kyxsanUserOptions.GetAccessTokenAsync().ConfigureAwait(false);
            kyxsanAsAServiceClient kyxsanAsAServiceClient = scope.ServiceProvider.GetRequiredService<kyxsanAsAServiceClient>();
            Response response = await kyxsanAsAServiceClient.UploadAnnouncementAsync(accessToken, Announcement).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                messenger.Send(InfoBarMessage.Success(response.Message));
                await taskContext.SwitchToMainThreadAsync();
                Announcement = new();
            }
        }
    }

    [Command("CompensationCdnServiceTimeCommand")]
    private async Task CompensationCdnServiceTimeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Compensation CDN service time", "TestViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            "kyxsan Cloud",
            $"Compensation CDN Service Time For {CdnCompensationDays} Days?").ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await kyxsanUserOptions.GetAccessTokenAsync().ConfigureAwait(false);
            kyxsanAsAServiceClient kyxsanAsAServiceClient = scope.ServiceProvider.GetRequiredService<kyxsanAsAServiceClient>();
            Response response = await kyxsanAsAServiceClient.CdnCompensationAsync(accessToken, CdnCompensationDays).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                messenger.Send(InfoBarMessage.Success(response.Message));
            }
        }
    }

    [Command("DesignationCdnServiceTimeCommand")]
    private async Task DesignationCdnServiceTimeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Designation CDN service time", "TestViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
            "kyxsan Cloud",
            $"Designation CDN Service Time To {CdnDesignationOptions.UserName} For {CdnDesignationOptions.Days} Days?").ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await kyxsanUserOptions.GetAccessTokenAsync().ConfigureAwait(false);
            kyxsanAsAServiceClient kyxsanAsAServiceClient = scope.ServiceProvider.GetRequiredService<kyxsanAsAServiceClient>();
            Response response = await kyxsanAsAServiceClient.CdnDesignationAsync(accessToken, CdnDesignationOptions.UserName, CdnDesignationOptions.Days).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider))
            {
                messenger.Send(InfoBarMessage.Success(response.Message));
            }
        }
    }

    [Command("SendRandomInfoBarNotificationCommand")]
    private void SendRandomInfoBarNotification()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Send random infobar notification", "TestViewModel.Command"));

        messenger.Send(new InfoBarMessage
        {
            Severity = (InfoBarSeverity)Random.Shared.Next((int)InfoBarSeverity.Error) + 1,
            Title = "Lorem ipsum dolor sit amet",
            Message = "Consectetur adipiscing elit. Nullam nec purus nec elit ultricies tincidunt. Donec nec sapien nec elit ultricies tincidunt. Donec nec sapien nec elit ultricies tincidunt.",
        });
    }

    [Command("CheckPathBelongsToSSDCommand")]
    private void CheckPathBelongsToSSD()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test SSD path", "TestViewModel.Command"));

        (bool isOk, ValueFile file) = fileSystemPickerInteraction.PickFile("Pick any file!", default, default);
        if (isOk)
        {
            bool isSolidState = PhysicalDrive.DangerousGetIsSolidState(file);
            messenger.Send(InfoBarMessage.Success($"The path '{file}' belongs to a {(isSolidState ? "solid state" : "hard disk")} drive."));
        }
    }

    [Command("RunCodeCommand")]
    private async Task RunCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test run code", "TestViewModel.Command"));

        try
        {
            string script = """
                            return 1 + 1;
                            """;
            object? result = await CSharpScript.EvaluateAsync(script, ScriptOptions.Default).ConfigureAwait(false);
            logger.LogInformation("Run Code Result: '{Result}'", result);
        }
        catch (CompilationErrorException ex)
        {
            logger.LogCritical(ex, "Compilation Error");
        }
    }

    [Command("ExtractGameBlocksCommand")]
    private async Task ExtractGameBlocksAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test extract game blocks", "TestViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGamePackageService gamePackageService = scope.ServiceProvider.GetRequiredService<IGamePackageService>();
            LaunchGameShared launchGameShared = scope.ServiceProvider.GetRequiredService<LaunchGameShared>();
            HoyoPlayClient hoyoPlayClient = scope.ServiceProvider.GetRequiredService<HoyoPlayClient>();
            LaunchOptions launchOptions = scope.ServiceProvider.GetRequiredService<LaunchOptions>();

            if (launchGameShared.GetCurrentLaunchSchemeFromConfigurationFile() is not { } launchScheme)
            {
                return;
            }

            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out GameBranchesWrapper? branchesWrapper))
            {
                return;
            }

            if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } gameBranch)
            {
                return;
            }

            if (gameBranch.PreDownload is null)
            {
                messenger.Send(InfoBarMessage.Warning("PreDownload not available"));
                return;
            }

            const string LockTrace = $"{nameof(TestViewModel)}.{nameof(ExtractGameBlocksAsync)}";
            if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
            {
                return;
            }

            ArgumentNullException.ThrowIfNull(gameFileSystem);
            using (gameFileSystem)
            {
                if (!gameFileSystem.TryGetGameVersion(out string? localVersion))
                {
                    return;
                }

                (bool isOk, string? extractDirectory) = fileSystemPickerInteraction.PickFolder("Select directory to extract the game blks");
                if (!isOk)
                {
                    return;
                }

                string message = $"""
                    Local: {localVersion}
                    Remote: {gameBranch.PreDownload.Tag}
                    Extract Directory: {extractDirectory}

                    Please ensure local game is integrated.
                    We need some of old blocks to patch up.
                    """;

                ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
                        "Extract Game Blocks",
                        message)
                    .ConfigureAwait(false);

                if (result is not ContentDialogResult.Primary)
                {
                    return;
                }

                BranchWrapper localBranch = gameBranch.Main.GetTaggedCopy(localVersion);
                BranchWrapper remoteBranch = gameBranch.PreDownload;

                ContentDialog dialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
                    .ConfigureAwait(false);

                SophonDecodedBuild? localBuild;
                SophonDecodedBuild? remoteBuild;
                SophonDecodedPatchBuild? patchBuild;
                using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    localBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, localBranch).ConfigureAwait(false);
                    remoteBuild = await gamePackageService.DecodeManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);
                    patchBuild = await gamePackageService.DecodeDiffManifestsAsync(gameFileSystem, remoteBranch).ConfigureAwait(false);
                    if (localBuild is null || remoteBuild is null || patchBuild is null)
                    {
                        messenger.Send(InfoBarMessage.Error(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
                        return;
                    }
                }

                GamePackageOperationContext context = new(serviceProvider, GamePackageOperationKind.ExtractBlocks, gameFileSystem, extractDirectory)
                {
                    LocalBuild = ExtractGameAssetBundles(localBuild),
                    RemoteBuild = ExtractGameAssetBundles(remoteBuild),
                    PatchBuild = ExtractGameAssetBundlesPatched(patchBuild),
                };

                await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false);
            }

            SophonDecodedBuild ExtractGameAssetBundles(SophonDecodedBuild decodedBuild)
            {
                SophonDecodedManifest manifest = decodedBuild.Manifests.First();
                SophonManifestProto proto = new();
                proto.Assets.AddRange(manifest.Data.Assets.Where(asset => AssetBundlesBlockRegex.IsMatch(asset.AssetName)));
                return new(decodedBuild.Tag, proto.Assets.Sum(a => a.AssetChunks.Sum(c => c.ChunkSize)), proto.Assets.Sum(a => a.AssetSize), [new(manifest.UrlPrefix, manifest.UrlSuffix, proto)]);
            }

            SophonDecodedPatchBuild ExtractGameAssetBundlesPatched(SophonDecodedPatchBuild patchBuild)
            {
                SophonDecodedPatchManifest manifest = patchBuild.Manifests.First();
                PatchManifest proto = new();
                proto.FileDatas.AddRange(manifest.Data.FileDatas.Where(fd => AssetBundlesBlockRegex.IsMatch(fd.FileName)));
                return new(patchBuild.OriginalTag, patchBuild.Tag, proto.FileDatas.Sum(fd => fd.PatchesEntries.Where(pe => pe.Key == patchBuild.OriginalTag).Sum(pe => pe.PatchInfo.PatchFileSize)), proto.FileDatas.Sum(fd => fd.PatchesEntries.Count(pe => pe.Key == patchBuild.OriginalTag)), proto.FileDatas.Where(fd => fd.PatchesEntries.SingleOrDefault(pe => pe.Key == patchBuild.OriginalTag) is not null).Sum(fd => fd.FileSize), proto.FileDatas.Count(fd => fd.PatchesEntries.SingleOrDefault(pe => pe.Key == patchBuild.OriginalTag) is not null), [new(patchBuild.OriginalTag, patchBuild.Tag, manifest.UrlPrefix, manifest.UrlSuffix, proto)]);
            }
        }
    }

    [Command("ExtractGameExeCommand")]
    private async Task ExtractGameExeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Test extract game executable", "TestViewModel.Command"));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGamePackageService gamePackageService = serviceProvider.GetRequiredService<IGamePackageService>();
            HoyoPlayClient hoyoPlayClient = serviceProvider.GetRequiredService<HoyoPlayClient>();

            LaunchScheme launchScheme = KnownLaunchSchemes.Values.First(s => s.IsOversea == ExtractExeOptions.IsOversea);

            Response<GameBranchesWrapper> branchResp = await hoyoPlayClient.GetBranchesAsync(launchScheme).ConfigureAwait(false);
            if (!ResponseValidator.TryValidate(branchResp, serviceProvider, out GameBranchesWrapper? branchesWrapper))
            {
                return;
            }

            if (branchesWrapper.GameBranches.FirstOrDefault(b => b.Game.Id == launchScheme.GameId) is not { } gameBranch)
            {
                return;
            }

            BranchWrapper? branch = ExtractExeOptions.IsPreDownload ? gameBranch.PreDownload : gameBranch.Main;

            if (branch is null)
            {
                messenger.Send(InfoBarMessage.Warning("PreDownload not available"));
                return;
            }

            if (fileSystemPickerInteraction.PickFolder("Select directory to extract the game blks") is not (true, { } extractDirectory))
            {
                return;
            }

            string message = $"""
                Version: {branch.Tag}
                IsOversea: {ExtractExeOptions.IsOversea}
                Extract Directory: {extractDirectory}
                """;

            ContentDialogResult result = await contentDialogFactory.CreateForConfirmCancelAsync(
                    "Extract Game Blocks",
                    message)
                .ConfigureAwait(false);

            if (result is ContentDialogResult.Primary)
            {
                IGameFileSystem gameFileSystem = GameFileSystem.CreateForPackageOperation(Path.Combine(extractDirectory, ExtractExeOptions.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName));

                ContentDialog dialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.UIXamlViewSpecializedSophonProgressDefault)
                    .ConfigureAwait(false);

                SophonDecodedBuild? build;
                using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
                {
                    build = await gamePackageService.DecodeManifestsAsync(gameFileSystem, branch).ConfigureAwait(false);
                    if (build is null)
                    {
                        messenger.Send(InfoBarMessage.Error(SH.ServiceGamePackageAdvancedDecodeManifestFailed));
                        return;
                    }
                }

                GamePackageOperationContext context = new(serviceProvider, GamePackageOperationKind.ExtractExecutable, gameFileSystem)
                {
                    RemoteBuild = ExtractGameExecutable(build),
                };

                await gamePackageService.ExecuteOperationAsync(context).ConfigureAwait(false);
            }

            SophonDecodedBuild ExtractGameExecutable(SophonDecodedBuild decodedBuild)
            {
                SophonDecodedManifest manifest = decodedBuild.Manifests.First();
                SophonManifestProto proto = new();
                proto.Assets.Add(manifest.Data.Assets.Single(a => GameExecutableFileRegex.IsMatch(a.AssetName)));
                return new(decodedBuild.Tag, proto.Assets.Sum(a => a.AssetChunks.Sum(c => c.ChunkSize)), proto.Assets.Sum(a => a.AssetSize), [new(manifest.UrlPrefix, manifest.UrlSuffix, proto)]);
            }
        }
    }

    [Command("GenerateRedeemCodeCommand")]
    private async Task GenerateRedeemCodeAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Generate redeem code", "TestViewModel.Command"));

        RedeemCodeType type = RedeemCodeType.None;
        if (RedeemCodeGenerateOption.IsTimeLimited)
        {
            type |= RedeemCodeType.TimeLimited;
        }

        if (RedeemCodeGenerateOption.IsTimesLimited)
        {
            type |= RedeemCodeType.TimesLimited;
        }

        if (type is RedeemCodeType.None)
        {
            messenger.Send(InfoBarMessage.Warning("Please select at least one type"));
            return;
        }

        if (RedeemCodeGenerateOption.ServiceType is RedeemCodeTargetServiceType.None)
        {
            messenger.Send(InfoBarMessage.Warning("Please select a service type"));
            return;
        }

        RedeemGenerateRequest request = new()
        {
            Count = RedeemCodeGenerateOption.Count,
            Type = type,
            ServiceType = RedeemCodeGenerateOption.ServiceType,
            Value = RedeemCodeGenerateOption.Value,
            Description = RedeemCodeGenerateOption.Description,
            ExpireTime = DateTimeOffset.UtcNow.AddHours(RedeemCodeGenerateOption.ExpireHours),
            Times = RedeemCodeGenerateOption.Times,
        };

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            string? accessToken = await kyxsanUserOptions.GetAccessTokenAsync().ConfigureAwait(false);
            kyxsanAsAServiceClient kyxsanAsAServiceClient = scope.ServiceProvider.GetRequiredService<kyxsanAsAServiceClient>();
            kyxsanResponse<RedeemGenerateResult> response = await kyxsanAsAServiceClient.GenerateRedeemCodesAsync(accessToken, request).ConfigureAwait(false);
            if (ResponseValidator.TryValidate(response, scope.ServiceProvider, out RedeemGenerateResult? generateResponse))
            {
                string message = $"""
                    {response.Message}
                    {string.Join(Environment.NewLine, generateResponse.Codes)}
                    Copied to clipboard
                    """;
                await clipboardProvider.SetTextAsync(string.Join(", ", generateResponse.Codes)).ConfigureAwait(false);
                messenger.Send(new InfoBarMessage
                {
                    Severity = InfoBarSeverity.Success,
                    Message = message,
                });
                await taskContext.SwitchToMainThreadAsync();
                RedeemCodeGenerateOption = new();
            }
        }
    }

    internal sealed class ExtractOptions
    {
        public bool IsOversea { get; set; }

        public bool IsPreDownload { get; set; }
    }

    internal sealed class DesignationOptions
    {
        public string UserName { get; set; } = default!;

        public int Days { get; set; }
    }

    internal sealed partial class RedeemCodeGenerateOptions : ObservableObject
    {
        public uint Count { get; set; }

        [ObservableProperty]
        public partial bool IsTimeLimited { get; set; }

        [ObservableProperty]
        public partial bool IsTimesLimited { get; set; }

        public RedeemCodeTargetServiceType ServiceType { get; set; }

        public int Value { get; set; }

        public string Description { get; set; } = default!;

        public int ExpireHours { get; set; }

        public uint Times { get; set; }
    }
}