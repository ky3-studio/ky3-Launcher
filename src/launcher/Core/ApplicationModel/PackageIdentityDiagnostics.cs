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

namespace Launcher.Core.ApplicationModel;

internal static class PackageIdentityDiagnostics
{
    public static void LogDiagnostics()
    {
        try
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ky3 Launcher",
                "startup_diagnostics.txt");

            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

            using (StreamWriter writer = File.CreateText(logPath))
            {
                writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Startup Diagnostics");
                writer.WriteLine($"HasPackageIdentity: {PackageIdentityAdapter.HasPackageIdentity}");
                writer.WriteLine($"AppVersion: {PackageIdentityAdapter.AppVersion}");
                writer.WriteLine($"AppDirectory: {PackageIdentityAdapter.AppDirectory}");
                writer.WriteLine($"FamilyName: {PackageIdentityAdapter.FamilyName}");
                writer.WriteLine($"PublisherId: {PackageIdentityAdapter.PublisherId}");
                writer.WriteLine("---");
            }

            Debug.WriteLine($"Diagnostics written to: {logPath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to write diagnostics: {ex.Message}");
        }
    }
}