//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using kyxsan.Core;
using kyxsan.Core.ApplicationModel;
using kyxsan.Core.Setting;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Service.Notification;
using System.IO;
using Windows.Storage;

namespace kyxsan.ViewModel.Setting;

internal sealed class SettingStorageSetDataFolderOperation
{
    public required IFileSystemPickerInteraction FileSystemPickerInteraction { private get; init; }

    public required IContentDialogFactory ContentDialogFactory { private get; init; }

    public required IMessenger Messenger { get; init; }

    internal async ValueTask<bool> TryExecuteAsync()
    {
        if (!FileSystemPickerInteraction.PickFolder().TryGetValue(out string? newFolderPath))
        {
            return false;
        }

        string oldFolderPath = kyxsanRuntime.DataDirectory;
        if (UrlPath.IsEqualOrSubdirectory(oldFolderPath, newFolderPath))
        {
            return false;
        }

        if (Path.GetDirectoryName(newFolderPath) is null)
        {
            await ContentDialogFactory.CreateForConfirmAsync(
                    SH.ViewModelSettingStorageSetDataFolderTitle,
                    SH.ViewModelSettingStorageSetDataFolderDescription2)
                .ConfigureAwait(false);

            return false;
        }

        Directory.CreateDirectory(newFolderPath);
        IEnumerable<string> entries;
        try
        {
            entries = Directory.EnumerateDirectories(newFolderPath);
        }
        catch (DirectoryNotFoundException)
        {
            return false;
        }

        if (entries.Any())
        {
            ContentDialogResult result = await ContentDialogFactory.CreateForConfirmCancelAsync(
                    SH.ViewModelSettingStorageSetDataFolderTitle,
                    SH.FormatViewModelSettingStorageSetDataFolderDescription3(newFolderPath))
                .ConfigureAwait(false);

            if (result is not ContentDialogResult.Primary)
            {
                return false;
            }
        }

        try
        {
            Directory.SetReadOnly(oldFolderPath, false);

            if (PackageIdentityAdapter.HasPackageIdentity)
            {
                StorageFolder oldFolder = await StorageFolder.GetFolderFromPathAsync(oldFolderPath);
                await oldFolder.CopyAsync(newFolderPath).ConfigureAwait(false);
            }
            else
            {
                await CopyDirectoryAsync(oldFolderPath, newFolderPath).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Messenger.Send(InfoBarMessage.Error(ex));
            return false;
        }

        LocalSetting.Set(SettingKeys.PreviousDataDirectoryToDelete, oldFolderPath);
        LocalSetting.Set(SettingKeys.DataDirectory, newFolderPath);
        LocalSetting.Set(SettingKeys.CacheDirectory, Path.Combine(newFolderPath, "Cache"));
        return true;
    }

    private static async ValueTask CopyDirectoryAsync(string sourceDir, string destDir)
    {
        await Task.Run(() =>
        {
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
            }

            foreach (string filePath in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(sourceDir, destDir), true);
            }
        }).ConfigureAwait(false);
    }
}