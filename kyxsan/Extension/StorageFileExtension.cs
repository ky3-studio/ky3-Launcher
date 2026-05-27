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

internal static class StorageFileExtension
{
    extension(StorageFile sourceFile)
    {
        public async ValueTask<StorageFile> CopyAsync(string targetFileFullPath, NameCollisionOption option = NameCollisionOption.ReplaceExisting)
        {
            StorageFolder targetFolder = await StorageFolder.GetFolderFromPathAsync(Path.GetDirectoryName(targetFileFullPath));
            return await sourceFile.CopyAsync(targetFolder, Path.GetFileName(targetFileFullPath), option);
        }
    }
}