//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.UI.Shell;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace kyxsan.Core.IO;

internal static class DirectoryOperation
{
    public static long GetSize(ValueDirectory directory, CancellationToken token = default)
    {
        if (!Directory.Exists(directory))
        {
            return 0;
        }

        long size = 0;
        try
        {
            foreach (string file in Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories))
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    size += new FileInfo(file).Length;
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }
        catch (Exception)
        {
            return 0;
        }

        return size;
    }

    public static bool Copy(ValueDirectory sourceDirectory, ValueDirectory targetDirectory, out Exception? exception)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            exception = new DirectoryNotFoundException($"Directory '{sourceDirectory}' not exists.");
            return false;
        }

        try
        {
            Microsoft.VisualBasic.FileIO.FileSystem.CopyDirectory(sourceDirectory, targetDirectory, true);
            exception = default;
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }

    public static bool Move(ValueDirectory sourceDirectory, ValueDirectory targetDirectory)
    {
        if (!Directory.Exists(sourceDirectory))
        {
            return false;
        }

        try
        {
            Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(sourceDirectory, targetDirectory, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void UnsafeRename(ValueDirectory directory, string name, FILEOPERATION_FLAGS flags = FILEOPERATION_FLAGS.FOF_ALLOWUNDO | FILEOPERATION_FLAGS.FOF_NOCONFIRMMKDIR)
    {
        FileSystem.RenameItem(directory, name, flags);
    }

    public static bool TrySetFullControl(ValueDirectory directory)
    {
        if (!Directory.Exists(directory))
        {
            return false;
        }

        if (WindowsIdentity.GetCurrent().User is not { } currentUser)
        {
            return false;
        }

        try
        {
            DirectoryInfo info = new(directory);
            DirectorySecurity accessControl = info.GetAccessControl();

            bool hasAllowedAccess = false;

            // Once we get access rules, it's cached, so we can safely enumerate and remove rules at the same time.
            foreach (FileSystemAccessRule rule in accessControl.GetAccessRules(true, true, typeof(SecurityIdentifier)))
            {
                if (rule.IdentityReference == currentUser)
                {
                    switch (rule.AccessControlType)
                    {
                        case AccessControlType.Deny:
                            accessControl.RemoveAccessRule(rule);
                            break;
                        case AccessControlType.Allow:
                            hasAllowedAccess = true;
                            break;
                    }
                }
            }

            if (hasAllowedAccess)
            {
                return true;
            }

            FileSystemAccessRule accessRule = new(
                currentUser,
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.InheritOnly,
                AccessControlType.Allow);
            accessControl.RemoveAccessRuleAll(accessRule);
            accessControl.AddAccessRule(accessRule);
            info.SetAccessControl(accessControl);

            return true;
        }
        catch
        {
            return false;
        }
    }
}