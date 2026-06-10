//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32;
using System.IO;

namespace kyxsan.Core.IO;

internal static class LogicalDrive
{
    public static long GetAvailableFreeSpace(ValueDirectory directory)
    {
        if (!directory.HasValue)
        {
            return 0;
        }

        string path = directory;
        if (!path.EndsWith('\\'))
        {
            path += '\\';
        }

        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? pathUri) && pathUri.IsUnc)
        {
            return kyxsanNative.Instance.MakeLogicalDrive().GetDiskFreeSpace(path);
        }

        try
        {
            string? root = Path.GetPathRoot(path);
            ArgumentException.ThrowIfNullOrWhiteSpace(root, "The path does not contain a root.");
            return new DriveInfo(root).AvailableFreeSpace;
        }
        catch (ArgumentException)
        {
            return kyxsanNative.Instance.MakeLogicalDrive().GetDiskFreeSpace(path);
        }
    }
}