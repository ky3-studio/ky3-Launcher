//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Microsoft.Windows.AppNotifications;
using kyxsan.Core.ApplicationModel;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.IO;
using kyxsan.Core.IO.Hashing;
using kyxsan.Core.Setting;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace kyxsan.Core;

internal static class kyxsanRuntime
{
    public static Version Version { get; } = PackageIdentityAdapter.AppVersion;

    public static string UserAgent { get; } = $"ky3 Launcher/{Version}";

    public static string DataDirectory { get; private set; } = InitializeDataDirectory();

    public static string LocalCacheDirectory { get; private set; } = InitializeLocalCacheDirectory();

    public static string FamilyName { get; } = PackageIdentityAdapter.FamilyName;

    public static string DeviceId { get; } = InitializeDeviceId();

    public static WebView2Version WebView2Version { get; } = InitializeWebView2();

    private static readonly Lazy<bool> LazyIsProcessElevated = new(GetIsProcessElevated);

    public static bool IsProcessElevated => LazyIsProcessElevated.Value;

    public static bool IsAppNotificationEnabled { get; } = CheckAppNotificationEnabled();

    public static string? GetDisplayName()
    {
        string name = new StringBuilder()
            .Append("App")
            .AppendIf(IsProcessElevated, "Elevated")
#if DEBUG
            .Append("Dev")
#endif
            .Append("NameAndVersion")
            .ToString();

        Debug.Assert(XamlApplicationLifetime.CultureInfoInitialized);
        string? displayName = SH.GetString(name, Version);
        return displayName is null ? null : string.Intern(displayName);
    }

    public static ValueFile GetDataDirectoryFile(string fileName)
    {
        return string.Intern(Path.Combine(DataDirectory, fileName));
    }

    public static ValueFile GetDataSubDirectoryFile(string subDir, string fileName)
    {
        string directory = Path.Combine(DataDirectory, subDir);
        Directory.CreateDirectory(directory);
        return string.Intern(Path.Combine(directory, fileName));
    }

    public static ValueFile GetDataUpdateCacheDirectoryFile(string fileName)
    {
        string directory = Path.Combine(DataDirectory, "UpdateCache");
        Directory.CreateDirectory(directory);
        return string.Intern(Path.Combine(directory, fileName));
    }

    public static ValueDirectory GetDataServerCacheDirectory()
    {
        string directory = Path.Combine(DataDirectory, "ServerCache");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    public static ValueDirectory GetDataScreenshotDirectory()
    {
        string directory = Path.Combine(DataDirectory, "Screenshot");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    public static ValueDirectory GetDataRepositoryDirectory()
    {
        string directory = Path.Combine(DataDirectory, "Repository");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    public static ValueDirectory GetLocalCacheImageCacheDirectory()
    {
        string directory = Path.Combine(LocalCacheDirectory, "ImageCache");
        Directory.CreateDirectory(directory);
        return string.Intern(directory);
    }

    internal static void SetDataDirectory(string path)
    {
        DataDirectory = path;
    }

    internal static void SetLocalCacheDirectory(string path)
    {
        LocalCacheDirectory = path;
    }

    private static bool GetIsProcessElevated()
    {
        try
        {
            return LocalSetting.Get(SettingKeys.OverrideElevationRequirement, false) || Environment.IsPrivilegedProcess;
        }
        catch
        {
            return Environment.IsPrivilegedProcess;
        }
    }

    private static string InitializeLocalCacheDirectory()
    {
        if (PackageIdentityAdapter.HasPackageIdentity)
        {
            return Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
        }

        string customPath = LocalSetting.Get(SettingKeys.CacheDirectory, string.Empty);
        if (!string.IsNullOrEmpty(customPath))
        {
            try
            {
                Directory.CreateDirectory(customPath);
                return customPath;
            }
            catch
            {
            }
        }

        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        const string FolderName
#if IS_ALPHA_BUILD
            = "ky3 Launcher Alpha";
#elif IS_CANARY_BUILD
            = "ky3 Launcher Canary";
#else
            = "ky3 Launcher";
#endif
        string cacheDir = Path.Combine(localAppData, FolderName, "Cache");
        Directory.CreateDirectory(cacheDir);
        return cacheDir;
    }

    private static bool CheckAppNotificationEnabled()
    {
        try
        {
            return AppNotificationManager.Default.Setting is AppNotificationSetting.Enabled;
        }
        catch
        {
            return false;
        }
    }

    private static string InitializeDataDirectory()
    {
        const string FolderName
#if IS_ALPHA_BUILD
        = "ky3 Launcher Alpha";
#elif IS_CANARY_BUILD
        = "ky3 Launcher Canary";
#else
        = "ky3 Launcher";
#endif

        const string OldFolderName
#if IS_ALPHA_BUILD
        = "kyxsanAlpha";
#elif IS_CANARY_BUILD
        = "kyxsanCanary";
#else
        = "kyxsan";
#endif

        string myDocumentsOldDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), OldFolderName));
        string myDocumentsNewDirectory = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), FolderName));
        if (Directory.Exists(myDocumentsOldDirectory) && !Directory.Exists(myDocumentsNewDirectory))
        {
            try
            {
                Directory.Move(myDocumentsOldDirectory, myDocumentsNewDirectory);
            }
            catch
            {
            }
        }

        string localApplicationData;
        if (PackageIdentityAdapter.HasPackageIdentity)
        {
            localApplicationData = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        }
        else
        {
            localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        string oldPath = Path.GetFullPath(Path.Combine(localApplicationData, OldFolderName));
        string defaultPath = Path.GetFullPath(Path.Combine(localApplicationData, FolderName));
        if (Directory.Exists(oldPath) && !Directory.Exists(defaultPath))
        {
            try
            {
                Directory.Move(oldPath, defaultPath);
            }
            catch
            {
            }
        }

        string prevDeleteDir = LocalSetting.Get(SettingKeys.PreviousDataDirectoryToDelete, string.Empty);
        if (!string.IsNullOrEmpty(prevDeleteDir))
        {
            CleanupOldDataDirectory(prevDeleteDir);
            LocalSetting.Set(SettingKeys.PreviousDataDirectoryToDelete, string.Empty);
        }

        string customPath = LocalSetting.Get(SettingKeys.DataDirectory, string.Empty);
        if (!string.IsNullOrEmpty(customPath))
        {
            try
            {
                Directory.CreateDirectory(customPath);

                if (!string.Equals(Path.GetFullPath(customPath), defaultPath, StringComparison.OrdinalIgnoreCase))
                {
                    CleanupOldDataDirectory(defaultPath);
                }

                MigrateRootFilesToSubfolders(customPath);
                return customPath;
            }
            catch
            {
            }
        }

        if (Directory.Exists(myDocumentsNewDirectory))
        {
            MigrateRootFilesToSubfolders(myDocumentsNewDirectory);
            return myDocumentsNewDirectory;
        }

        try
        {
            Directory.CreateDirectory(defaultPath);
        }
        catch (Exception ex)
        {
            kyxsanException.InvalidOperation($"Failed to create data folder: {defaultPath}", ex);
        }

        MigrateRootFilesToSubfolders(defaultPath);
        return defaultPath;
    }

    private static void MigrateRootFilesToSubfolders(string dataDir)
    {
        MoveFileIfExists(dataDir, "Userdata.db", "Data");
        MoveFileIfExists(dataDir, "Userdata.db-journal", "Data");
        MoveFileIfExists(dataDir, "Userdata.db-wal", "Data");
        MoveFileIfExists(dataDir, "Userdata.db-shm", "Data");
        MoveFileIfExists(dataDir, "ShellLinkLogo.ico", "Assets");
        MoveFileIfExists(dataDir, "YaeAchievementLib.dll", "Lib");
    }

    private static void MoveFileIfExists(string baseDir, string fileName, string subDir)
    {
        string oldPath = Path.Combine(baseDir, fileName);
        if (!File.Exists(oldPath))
        {
            return;
        }

        string newDir = Path.Combine(baseDir, subDir);
        Directory.CreateDirectory(newDir);
        string newPath = Path.Combine(newDir, fileName);
        if (!File.Exists(newPath))
        {
            try
            {
                File.Move(oldPath, newPath);
            }
            catch
            {
            }
        }
    }

    private static void CleanupOldDataDirectory(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            return;
        }

        string[] dataSubdirectories = ["Repository", "ServerCache", "Screenshot", "UpdateCache", "Cache", "Data", "Assets", "Lib"];
        foreach (string subDir in dataSubdirectories)
        {
            try
            {
                string fullPath = Path.Combine(dirPath, subDir);
                if (Directory.Exists(fullPath))
                {
                    Directory.Delete(fullPath, true);
                }
            }
            catch
            {
            }
        }

        try
        {
            foreach (string file in Directory.EnumerateFiles(dirPath, "Userdata.db*"))
            {
                File.Delete(file);
            }
        }
        catch
        {
        }

        string[] rootFiles = ["ShellLinkLogo.ico", "YaeAchievementLib.dll"];
        foreach (string fileName in rootFiles)
        {
            try
            {
                string filePath = Path.Combine(dirPath, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }
        }
    }

    private static string InitializeDeviceId()
    {
        string userName = Environment.UserName;
        object? machineGuid = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography\", "MachineGuid", userName);
        return Hash.ToHexString(HashAlgorithmName.MD5, $"{userName}{machineGuid}");
    }

    private static WebView2Version InitializeWebView2()
    {
        try
        {
            string version = CoreWebView2Environment.GetAvailableBrowserVersionString();
            return new(version, version, true);
        }
        catch (FileNotFoundException)
        {
            return new(string.Empty, SH.CoreWebView2HelperVersionUndetected, false);
        }
    }
}