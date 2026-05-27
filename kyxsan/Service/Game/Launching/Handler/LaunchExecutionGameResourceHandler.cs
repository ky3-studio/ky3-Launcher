//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Win32.SafeHandles;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.Setting;
using kyxsan.Factory.ContentDialog;
using kyxsan.Model.Intrinsic;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Launching.Context;
using kyxsan.Service.Game.Package;
using kyxsan.Service.Game.Package.Advanced;
using kyxsan.Service.Notification;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.Branch;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.ChannelSDK;
using kyxsan.Web.Hoyolab.HoyoPlay.Connect.DeprecatedFile;
using System.IO;
using System.Net.Http;

namespace kyxsan.Service.Game.Launching.Handler;

internal sealed class LaunchExecutionGameResourceHandler : AbstractLaunchExecutionHandler
{
    private readonly bool convertOnly;

    public LaunchExecutionGameResourceHandler(bool convertOnly)
    {
        this.convertOnly = convertOnly;
    }

    public override async ValueTask BeforeAsync(BeforeLaunchExecutionContext context)
    {
        if (!ShouldConvert(context))
        {
            if (convertOnly)
            {
                context.Messenger.Send(InfoBarMessage.Information(SH.ServiceGameLaunchingEnsureGameResourceShouldNotConvert));
            }

            return;
        }

        using (BlockDeferral<PackageConvertStatus> deferral = await context.ViewModel.CreateConvertBlockDeferralAsync().ConfigureAwait(false))
        {
            await EnsureGameResourceAsync(context, deferral.Progress).ConfigureAwait(false);
        }
    }

    private static bool ShouldConvert(BeforeLaunchExecutionContext context)
    {
        // Configuration file changed
        if (context.TryGetOption(LaunchExecutionOptionsKey.ChannelOptionsChanged, out bool changed) && changed)
        {
            return true;
        }

        if (context.TargetScheme.IsOversea ^ context.FileSystem.IsExecutableOversea)
        {
            return true;
        }

        if (!context.TargetScheme.IsOversea)
        {
            // [It's Bilibili channel xor PCGameSDK.dll exists] means we need to convert
            if (context.TargetScheme.Channel is ChannelType.Bili ^ File.Exists(context.FileSystem.PCGameSDKFilePath))
            {
                return true;
            }
        }

        return false;
    }

    private static async ValueTask EnsureGameResourceAsync(BeforeLaunchExecutionContext context, IProgress<PackageConvertStatus> progress)
    {
        string gameFolder = context.FileSystem.GameDirectory;

        if (!CheckDirectoryPermissions(gameFolder, out Exception? inner))
        {
            throw kyxsanException.UnauthorizedAccess(SH.ServiceGameEnsureGameResourceInsufficientDirectoryPermissions, inner);
        }

        progress.Report(new(SH.ServiceGameEnsureGameResourceQueryResourceInformation));

        ValueTask<ValueResult<bool, GameBranchesWrapper>> currentBranchesTask = context.HoyoPlay.TryGetBranchesAsync(context.CurrentScheme);
        ValueTask<ValueResult<bool, GameBranchesWrapper>> targetBranchesTask = context.HoyoPlay.TryGetBranchesAsync(context.TargetScheme);
        ValueTask<ValueResult<bool, GameChannelSDKsWrapper>> channelSdksTask = context.HoyoPlay.TryGetChannelSDKsAsync(context.TargetScheme);
        ValueTask<ValueResult<bool, DeprecatedFileConfigurationsWrapper>> deprecatedTask = context.HoyoPlay.TryGetDeprecatedFileConfigurationsAsync(context.TargetScheme);

        if (await currentBranchesTask.ConfigureAwait(false) is not (true, { } currentBranches))
        {
            throw kyxsanException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Current Branches"));
        }

        if (await targetBranchesTask.ConfigureAwait(false) is not (true, { } targetBranches))
        {
            throw kyxsanException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Branches"));
        }

        if (await channelSdksTask.ConfigureAwait(false) is not (true, { } channelSdks))
        {
            throw kyxsanException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Channel SDKs"));
        }

        if (await deprecatedTask.ConfigureAwait(false) is not (true, { } deprecatedFileConfigs))
        {
            throw kyxsanException.InvalidOperation(SH.FormatServiceGameLaunchExecutionGameResourceQueryIndexFailed("Target Deprecated File Configs"));
        }

        IHttpClientFactory httpClientFactory = context.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using (HttpClient httpClient = httpClientFactory.CreateClient(GamePackageService.HttpClientName))
        {
            PackageConverterContext converterContext = new(context.CurrentScheme, context.TargetScheme, context.FileSystem)
            {
                HttpClient = httpClient,
                Progress = progress,
                CurrentBranch = currentBranches.GetMainBranch(context.CurrentScheme.GameId),
                TargetBranch = targetBranches.GetMainBranch(context.TargetScheme.GameId),
            };

            IPackageConverter packageConverter = context.ServiceProvider.GetRequiredService<IPackageConverter>();

            if (context.TargetScheme.IsOversea ^ context.FileSystem.IsExecutableOversea)
            {
                if (!await packageConverter.EnsureGameResourceAsync(converterContext).ConfigureAwait(false))
                {
                    throw kyxsanException.InvalidOperation(SH.ViewModelLaunchGameEnsureGameResourceFail);
                }

                await context.TaskContext.SwitchToMainThreadAsync();
                string executableName = context.TargetScheme.IsOversea ? GameConstants.GenshinImpactFileName : GameConstants.YuanShenFileName;
                string newGamePath = Path.Combine(gameFolder, executableName);
                context.FileSystem = context.LaunchOptions.UnsafeForceUpdateGamePath(newGamePath, context.FileSystem);
            }

            PackageConverterDeprecationContext deprecationContext = new(httpClient, context.FileSystem, channelSdks.GameChannelSDKs.SingleOrDefault(), deprecatedFileConfigs.DeprecatedFileConfigurations.SingleOrDefault());
            await packageConverter.EnsureDeprecatedFilesAndSDKAsync(deprecationContext).ConfigureAwait(false);
        }
    }

    private static bool CheckDirectoryPermissions(string folder, [NotNullWhen(false)] out Exception? exception)
    {
        if (!LocalSetting.Get(SettingKeys.LaunchOverridePackageConvertDirectoryPermissions, false))
        {
            try
            {
                Directory.CreateDirectory(folder);

                string tempFilePath = Path.Combine(folder, $"{Guid.NewGuid():N}.tmp");
                string tempFilePathMove = Path.Combine(folder, $"{Guid.NewGuid():N}.tmp");

                // Test create file
                using (SafeFileHandle handle = File.OpenHandle(tempFilePath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None, preallocationSize: 32 * 1024))
                {
                    // Test write file
                    RandomAccess.Write(handle, "KYXSAN DIRECTORY PERMISSION CHECK"u8, 0);
                    RandomAccess.FlushToDisk(handle);
                }

                // Test move file
                File.Move(tempFilePath, tempFilePathMove);

                // Test delete file
                File.Delete(tempFilePathMove);
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }

        exception = null;
        return true;
    }
}