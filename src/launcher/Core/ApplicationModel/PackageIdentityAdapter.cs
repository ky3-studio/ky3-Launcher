// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace kyxsan.Core.ApplicationModel;

internal static class PackageIdentityAdapter
{
    private static readonly Lazy<bool> LazyHasPackageIdentity = new(CheckPackageIdentity);
    private static readonly Lazy<string> LazyAppDirectory = new(GetAppDirectoryPath);
    private static readonly Lazy<Version> LazyAppVersion = new(GetAppVersionInternal);
    private static readonly Lazy<string> LazyFamilyName = new(GetFamilyNameInternal);
    private static readonly Lazy<string> LazyPublisherId = new(GetPublisherIdInternal);

    public static bool HasPackageIdentity => LazyHasPackageIdentity.Value;

    public static string AppDirectory => LazyAppDirectory.Value;

    public static Version AppVersion => LazyAppVersion.Value;

    public static string FamilyName => LazyFamilyName.Value;

    public static string PublisherId => LazyPublisherId.Value;

    private static bool CheckPackageIdentity()
    {
        try
        {
            _ = Windows.ApplicationModel.Package.Current.Id;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string GetAppDirectoryPath()
    {
        if (HasPackageIdentity)
        {
            return Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        }

        string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
        ArgumentException.ThrowIfNullOrEmpty(exePath);
        string? directory = Path.GetDirectoryName(exePath);
        ArgumentException.ThrowIfNullOrEmpty(directory);
        return directory;
    }

    private static Version GetAppVersionInternal()
    {
        static Version TrimToThree(Version v)
        {
            int major = v.Major;
            int minor = v.Minor;
            int build = v.Build >= 0 ? v.Build : 0;
            return new Version(major, minor, build);
        }

        if (HasPackageIdentity)
        {
            Version v = Windows.ApplicationModel.Package.Current.Id.Version.ToVersion();
            return TrimToThree(v);
        }

        // .csproj 定义，勿改
        Assembly assembly = Assembly.GetExecutingAssembly();
        Version? version = assembly.GetName().Version;
        return version is not null ? TrimToThree(version) : new Version(1, 0, 0);
    }

    private static string GetFamilyNameInternal()
    {
        if (HasPackageIdentity)
        {
            return Windows.ApplicationModel.Package.Current.Id.FamilyName;
        }

        return "kyxsan.Unpackaged";
    }

    private static string GetPublisherIdInternal()
    {
        if (HasPackageIdentity)
        {
            return Windows.ApplicationModel.Package.Current.Id.PublisherId;
        }

        return "CN=Millennium-Science-Technology-R-D-Inst";
    }
}
