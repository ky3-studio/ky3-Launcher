//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using kyxsan.Core.IO;
using System.IO;

namespace kyxsan.Core.Shell;

[Service(ServiceLifetime.Transient, typeof(IShellLinkInterop))]
internal sealed class ShellLinkInterop : IShellLinkInterop
{
    public bool TryCreateDesktopShortcutForElevatedLaunch()
    {
        string targetLogoPath = kyxsanRuntime.GetDataSubDirectoryFile("Assets", "ShellLinkLogo.ico");
        string elevatedLauncherPath = Environment.ProcessPath ?? string.Empty;

        try
        {
            InstalledLocation.CopyFileFromApplicationUri("ms-appx:///Assets/Logo.ico", targetLogoPath);
            // Moved for unpackaged deployment
            // InstalledLocation.CopyFileFromApplicationUri("ms-appx:///kyxsan.Elevated.Launcher.exe", elevatedLauncherPath);

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string target = Path.Combine(desktop, $"{SH.AppName}.lnk");

            // Always point the shortcut to the elevated launcher executable and pass FamilyName as argument.
            // The elevated launcher will interpret the argument to activate packaged app when appropriate.
            FileSystem.CreateLink(elevatedLauncherPath, String.Empty, targetLogoPath, target);

            return true;
        }
        catch
        {
            return false;
        }
    }
}