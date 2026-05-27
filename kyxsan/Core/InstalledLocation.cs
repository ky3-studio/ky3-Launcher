//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ApplicationModel;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace kyxsan.Core;

internal static class InstalledLocation
{
    public static string GetAbsolutePath(string relativePath)
    {
        return Path.Combine(PackageIdentityAdapter.AppDirectory, relativePath);
    }

    public static void CopyFileFromApplicationUri(string url, string path)
    {
        CopyApplicationUriFileCoreAsync(url, path).GetAwaiter().GetResult();

        static async Task CopyApplicationUriFileCoreAsync(string url, string path)
        {
            await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

            Uri uri = url.ToUri();
            Stream outputStream;

            if (PackageIdentityAdapter.HasPackageIdentity)
            {
                Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(uri);
                outputStream = (await file.OpenReadAsync()).AsStreamForRead();
            }
            else
            {
                string localPath = uri.LocalPath.TrimStart('/');
                string fullPath = Path.Combine(PackageIdentityAdapter.AppDirectory, localPath);
                outputStream = File.OpenRead(fullPath);
            }

            using (outputStream)
            {
                if (File.Exists(path))
                {
                    try
                    {
                        FileInfo fileInfo = new(path);
                        FileSecurity fileSecurity = fileInfo.GetAccessControl();
                        SecurityIdentifier? user = WindowsIdentity.GetCurrent().User;

                        if (user is not null)
                        {
                            fileSecurity.AddAccessRule(new(user, FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                            fileInfo.SetAccessControl(fileSecurity);
                        }
                    }
                    catch
                    {
                        // Ignore
                    }
                }

                // 确保目标目录存在
                string? directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (FileStream inputStream = File.Create(path))
                {
                    await outputStream.CopyToAsync(inputStream).ConfigureAwait(false);
                }
            }
        }
    }
}