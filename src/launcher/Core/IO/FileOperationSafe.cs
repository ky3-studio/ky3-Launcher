//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.Logging;
using System.IO;

namespace Launcher.Core.IO;

internal static class FileOperationSafe
{
    public static bool TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return true;
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "File delete failed",
                "FileOperationSafe",
                [("Path", path), ("Error", ex.Message)]));
            return false;
        }
    }

    public static bool TryMove(string source, string destination, bool overwrite = false)
    {
        try
        {
            if (!File.Exists(source))
            {
                return false;
            }

            File.Move(source, destination, overwrite);
            return true;
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "File move failed",
                "FileOperationSafe",
                [("Source", source), ("Destination", destination), ("Error", ex.Message)]));
            return false;
        }
    }

    public static bool TryWriteAllText(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content);
            return true;
        }
        catch (Exception ex)
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateError(
                "File write failed",
                "FileOperationSafe",
                [("Path", path), ("Error", ex.Message)]));
            return false;
        }
    }
}
