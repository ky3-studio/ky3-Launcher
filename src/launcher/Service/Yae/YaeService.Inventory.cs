//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___          __   __ _    _____
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \  __  __ \ \ / // \  | ____|
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | | \ \/ /  \ V // _ \ |  _|
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |  >  <    | |/ ___ \| |___
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/  /_/\_\   |_/_/   \_\_____|
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Launcher.Core;
using Launcher.Core.Diagnostics;
using Launcher.Core.LifeCycle.InterProcess.Yae;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Game;
using Launcher.Service.Game.FileSystem;
using Launcher.Service.Notification;
using Launcher.Service.Yae.Achievement;
using Launcher.Service.Yae.PlayerStore;
using System.Collections.Immutable;
using System.IO;

namespace Launcher.Service.Yae;

internal sealed partial class YaeService
{
    public async ValueTask<ImmutableArray<(uint ItemId, uint Count)>?> GetInventoryAsync()
    {
        if (!LauncherRuntime.IsProcessElevated)
        {
            messenger.Send(InfoBarMessage.Error(SH.ServiceGameLaunchingHandlerEmbeddedYaeClientNotElevated));
            return default;
        }

        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ServiceYaeWaitForGameResponseMessage)
            .ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        dialog.Title = null;
        dialog.Content = new StackPanel
        {
            Spacing = 16,
            Children =
            {
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 16,
                    Children =
                    {
                        new ProgressRing { IsActive = true, Width = 32, Height = 32 },
                        new TextBlock
                        {
                            Text = SH.ServiceYaeWaitForGameResponseMessage,
                            VerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment.Center,
                            FontSize = 16,
                        },
                    },
                },
                new ProgressBar { IsIndeterminate = true },
                new TextBlock
                {
                    Text = SH.ServiceYaeInventoryImportHint,
                    FontSize = 12,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray),
                    TextWrapping = Microsoft.UI.Xaml.TextWrapping.Wrap,
                },
            },
        };

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            await taskContext.SwitchToBackgroundAsync();
            try
            {
                LaunchOptions launchOptions = serviceProvider.GetRequiredService<LaunchOptions>();
                const string LockTrace = $"{nameof(YaeService)}.{nameof(GetInventoryAsync)}";

                if (launchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None || gameFileSystem is null)
                {
                    messenger.Send(InfoBarMessage.Error(SH.ServiceYaeGetGameVersionFailed));
                    return default;
                }

                string gameFilePath;
                string gameDirectory;
                TargetNativeConfiguration config;

                using (gameFileSystem)
                {
                    gameFilePath = gameFileSystem.GameFilePath;
                    gameDirectory = gameFileSystem.GameDirectory;

                    YaeAchievementInfo? metadata = await yaeCdnClient.GetMetadataAsync().ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(metadata, "Failed to fetch Yae metadata from CDN");
                    ArgumentNullException.ThrowIfNull(metadata.NativeConfig);

                    uint gameHash = GetGameHash(gameFilePath);

                    if (!metadata.NativeConfig.MethodRva.TryGetValue(gameHash, out YaeMethodRvaConfig? methodRva))
                    {
                        messenger.Send(InfoBarMessage.Error($"Unsupported game version (hash: 0x{gameHash:X8})"));
                        return default;
                    }

                    config = new()
                    {
                        StoreCmdId = metadata.NativeConfig.StoreCmdId,
                        AchievementCmdId = metadata.NativeConfig.AchievementCmdId,
                        DoCmd = methodRva.DoCmd,
                        UpdateNormalProperty = methodRva.UpdateNormalProp,
                        NewString = methodRva.NewString,
                        FindGameObject = methodRva.FindGameObject,
                        EventSystemUpdate = methodRva.EventSystemUpdate,
                        SimulatePointerClick = methodRva.SimulatePointerClick,
                        ToInt32 = methodRva.ToInt32,
                        TcpStatePtr = methodRva.TcpStatePtr,
                        SharedInfoPtr = methodRva.SharedInfoPtr,
                        Decompress = methodRva.Decompress,
                    };
                }

                string srcDllPath = LauncherRuntime.GetDataSubDirectoryFile("Lib", "YaeAchievementLib.dll");
                InstalledLocation.CopyFileFromApplicationUri("ms-appx:///YaeAchievementLib.dll", srcDllPath);

                string dllPath = Path.Combine(gameDirectory, "YaeAchievementLib.dll");
                File.Copy(srcDllPath, dllPath, overwrite: true);

                if (!File.Exists(dllPath))
                {
                    throw new FileNotFoundException("YaeAchievementLib.dll not found", dllPath);
                }

                ImmutableArray<YaeData> dataArray = await InjectAndCollectAsync(
                    gameFilePath, gameDirectory, dllPath, config, launchOptions).ConfigureAwait(false);

                ImmutableArray<(uint ItemId, uint Count)>? result = default;
                Dictionary<InterestedPropType, double> propMap = [];
                foreach (YaeData data in dataArray)
                {
                    using (data)
                    {
                        switch (data.Kind)
                        {
                            case YaeCommandKind.ResponsePlayerStore:
                                result = PlayerStoreParser.Parse(data.Bytes);
                                break;
                            case YaeCommandKind.ResponsePlayerProp:
                                ref readonly YaePropertyTypeValue typeValue = ref data.PropertyTypeValue;
                                propMap[typeValue.Type] = typeValue.Value;
                                break;
                        }
                    }
                }

                if (result is null)
                {
                    return default;
                }

                double moraCount = propMap.GetValueOrDefault(InterestedPropType.PlayerSCoin)
                    - propMap.GetValueOrDefault(InterestedPropType.PlayerWaitSubSCoin);
                if (moraCount > 0)
                {
                    result = result.Value.Add((202U, (uint)Math.Clamp(moraCount, uint.MinValue, uint.MaxValue)));
                }

                return result;
            }
            catch (Exception ex)
            {
                messenger.Send(InfoBarMessage.Error(ex));
                return default;
            }
            finally
            {
                await GameLifeCycle.IsGameRunningAsync(taskContext).ConfigureAwait(false);
            }
        }
    }
}
