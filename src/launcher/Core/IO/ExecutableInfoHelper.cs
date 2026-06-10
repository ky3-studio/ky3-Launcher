// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace kyxsan.Core.IO;

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