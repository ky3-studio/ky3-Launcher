//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.IO;
using Windows.Storage;

namespace kyxsan.Extension;

internal static class StorageFolderExtension
{
    extension(StorageFolder sourceFolder)
    {
        public async ValueTask<StorageFolder> CopyAsync(string targetFolderFullPath,
            NameCollisionOption nameCollisionOption = NameCollisionOption.ReplaceExisting,
            CreationCollisionOption creationCollisionOption = CreationCollisionOption.ReplaceExisting)
        {
            Directory.CreateDirectory(targetFolderFullPath);
            StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(targetFolderFullPath);
            await sourceFolder.CopyAsync(targetFolder, nameCollisionOption, creationCollisionOption).ConfigureAwait(false);
            return targetFolder;
        }

        public async ValueTask CopyAsync(StorageFolder targetFolder,
            NameCollisionOption nameCollisionOption = NameCollisionOption.ReplaceExisting,
            CreationCollisionOption creationCollisionOption = CreationCollisionOption.OpenIfExists)
        {
            foreach (StorageFolder folder in await sourceFolder.GetFoldersAsync())
            {
                StorageFolder subFolder = await targetFolder.CreateFolderAsync(folder.Name, creationCollisionOption);
                await folder.CopyAsync(subFolder).ConfigureAwait(false);
            }

            foreach (StorageFile file in await sourceFolder.GetFilesAsync())
            {
                await file.CopyAsync(targetFolder, file.Name, nameCollisionOption);
            }
        }
    }
}