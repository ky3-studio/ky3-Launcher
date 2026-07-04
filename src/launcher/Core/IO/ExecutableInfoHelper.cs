//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Launcher.Core.IO;

internal static class ExecutableInfoHelper
{
    public static string GetFriendlyName(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Executable not found.", path);
        }

        try
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);

            if (!string.IsNullOrWhiteSpace(versionInfo.ProductName))
            {
                return versionInfo.ProductName;
            }

            if (!string.IsNullOrWhiteSpace(versionInfo.FileDescription))
            {
                return versionInfo.FileDescription;
            }
        }
        catch
        {
        }

        try
        {
            AssemblyName asmName = AssemblyName.GetAssemblyName(path);
            if (!string.IsNullOrWhiteSpace(asmName.Name))
            {
                return asmName.Name;
            }
        }
        catch
        {
        }

        return Path.GetFileNameWithoutExtension(path) ?? path;
    }
}