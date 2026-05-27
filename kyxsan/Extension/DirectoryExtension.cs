//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.IO;

namespace kyxsan.Extension;

internal static class DirectoryExtension
{
    extension(Directory)
    {
        public static void SetReadOnly(string path, bool isReadOnly)
        {
            DirectoryInfo dirInfo = new(path);
            dirInfo.Attributes = isReadOnly
                ? dirInfo.Attributes | FileAttributes.ReadOnly
                : dirInfo.Attributes & ~FileAttributes.ReadOnly;

            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                fileInfo.Attributes = isReadOnly
                    ? fileInfo.Attributes | FileAttributes.ReadOnly
                    : fileInfo.Attributes & ~FileAttributes.ReadOnly;
            }

            foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
            {
                SetReadOnly(subDirInfo.FullName, isReadOnly);
            }
        }
    }
}