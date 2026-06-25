//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using kyxsan.Core;
using kyxsan.Core.ApplicationModel;
using kyxsan.Core.LifeCycle;
using kyxsan.Core.Logging;
using kyxsan.Service;
using kyxsan.Service.BackgroundActivity;
using kyxsan.Service.Metadata;
using kyxsan.Service.Notification;
using kyxsan.Service.RedeemCode;
using kyxsan.Service.RemoteConfig;
using kyxsan.Core.IO.Http.Proxy;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.Update;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;

namespace kyxsan.ViewModel;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Transient)]
internal sealed partial class MainViewModel : Abstraction.ViewModel, IDisposable
{
    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IMetadataService metadataService;
    private readonly IUpdateService updateService;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    internal static readonly SemaphoreSlim s_patchDownloadLock = new(1, 1);

    [GeneratedConstructor]
    public partial MainViewModel(IServiceProvider serviceProvider);

    public static string? Title { get => kyxsanRuntime.GetDisplayName(); }

    [ObservableProperty]
    public partial bool IsUpdateReady { get; set; }

    [ObservableProperty]
    public partial int UpdateReadyNotifyToken { get; set; }

    public partial AppOptions AppOptions { get; }

    public partial BackgroundActivityOptions BackgroundActivityOptions { get; }

    public partial RedeemCodeService RedeemCodeService { get; }

    public override void Dispose()
    {
        using (CriticalSection.Enter())
        {
            Uninitialize();
        }

        base.Dispose();
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDataFolderHasReparsePoint();

        _ = RedeemCodeService.RefreshAsync();
        _ = CheckAndPrepareUpdateAsync();
        _ = StartPeriodicUpdateCheckAsync();

        return true;
    }

    [Command("RestartToUpdateCommand")]
    private void RestartToUpdate()
    {
        try
        {
            string updateFilesDir = Path.Combine(kyxsanRuntime.DataDirectory, "UpdateCache", "files");
            if (!Directory.Exists(updateFilesDir))
            {
                return;
            }

            string appDir = PackageIdentityAdapter.AppDirectory;

            // Prefer the new updater from the patch cache (so old installations get recursive copy support)
            string patchUpdaterPath = Path.Combine(updateFilesDir, "updater.exe");
            string installedUpdaterPath = Path.Combine(appDir, "updater.exe");
            string updaterPath = File.Exists(patchUpdaterPath) ? patchUpdaterPath : installedUpdaterPath;

            if (!File.Exists(updaterPath))
            {
                return;
            }

            int pid = Environment.ProcessId;
            string exeName = Path.GetFileName(Process.GetCurrentProcess().MainModule?.FileName ?? "launcher.exe");

            Process.Start(new ProcessStartInfo
            {
                FileName = updaterPath,
                Arguments = $"--pid {pid} --source \"{updateFilesDir}\" --target \"{appDir}\" --exe \"{exeName}\"",
                UseShellExecute = true,
            });

            Application.Current.Exit();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    private async Task StartPeriodicUpdateCheckAsync()
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(30));
        while (await timer.WaitForNextTickAsync().ConfigureAwait(false))
        {
            if (IsUpdateReady)
            {
                break;
            }

            await CheckAndPrepareUpdateAsync(skipDelay: true).ConfigureAwait(false);
        }
    }

    private async Task CheckAndPrepareUpdateAsync(bool skipDelay = false)
    {
        try
        {
            if (!skipDelay)
            {
                await Task.Delay(3000).ConfigureAwait(false);
            }

            CheckUpdateResult result = await updateService.CheckUpdateAsync().ConfigureAwait(false);

            if (result.Kind is not CheckUpdateResultKind.UpdateAvailable)
            {
                return;
            }

            if (result.PackageInformation is null)
            {
                return;
            }

            // Mandatory update: show dialog, user cannot skip
            await taskContext.SwitchToMainThreadAsync();
            ContentDialogResult dialogResult = await contentDialogFactory.CreateForConfirmAsync(
                SH.ViewModelMainUpdateMandatoryTitle,
                SH.FormatViewModelMainUpdateMandatoryContent(result.PackageInformation.Version.ToString()));

            if (dialogResult is not ContentDialogResult.Primary)
            {
                Application.Current.Exit();
                return;
            }

            // Download and extract incremental patch
            if (string.IsNullOrEmpty(result.PackageInformation.PatchUrl) ||
                string.IsNullOrEmpty(result.PackageInformation.PatchSha256))
            {
                messenger.Send(InfoBarMessage.Warning(SH.ViewModelMainUpdatePatchNotAvailable));
                Application.Current.Exit();
                return;
            }

            bool patchSuccess = await DownloadAndExtractPatchAsync(result.PackageInformation).ConfigureAwait(false);
            if (patchSuccess)
            {
                await taskContext.SwitchToMainThreadAsync();
                RestartToUpdate();
            }
            else
            {
                await taskContext.SwitchToMainThreadAsync();
                messenger.Send(InfoBarMessage.Warning(SH.ViewModelMainUpdateDownloadFailed));
                Application.Current.Exit();
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    internal static async Task<bool> DownloadAndExtractPatchAsync(Web.kyxsan.kyxsanPackageInformation packageInfo, CancellationToken token = default)
    {
        await s_patchDownloadLock.WaitAsync(token).ConfigureAwait(false);
        try
        {
            string updateCacheDir = Path.Combine(kyxsanRuntime.DataDirectory, "UpdateCache");
            string filesDir = Path.Combine(updateCacheDir, "files");
            string versionMarker = Path.Combine(updateCacheDir, "version.txt");

            // If already extracted for this version, skip
            if (Directory.Exists(filesDir) && File.Exists(versionMarker))
            {
                string cachedVersion = await File.ReadAllTextAsync(versionMarker, token).ConfigureAwait(false);
                if (cachedVersion.Trim() == packageInfo.Version.ToString())
                {
                    return true;
                }
            }

            // Clean previous cache
            if (Directory.Exists(filesDir))
            {
                Directory.Delete(filesDir, true);
            }

            Directory.CreateDirectory(updateCacheDir);

            // Build full download URL
            string patchUrl = packageInfo.PatchUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? packageInfo.PatchUrl
                : $"{BackendApiRoutes.ServerRoot}{packageInfo.PatchUrl}";

            string patchZipPath = Path.Combine(updateCacheDir, "patch.zip");

            // Download patch.zip with proxy support
            SocketsHttpHandler handler = new()
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true,
                },
                Proxy = HttpProxyUsingSystemProxy.Instance,
                UseProxy = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            };

            using HttpClient client = new(handler) { Timeout = TimeSpan.FromMinutes(5) };

            using HttpResponseMessage response = await client.GetAsync(patchUrl.ToUri(), HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            using (Stream netStream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
            using (FileStream fileStream = File.Create(patchZipPath))
            {
                await netStream.CopyToAsync(fileStream, token).ConfigureAwait(false);
            }

            // SHA256 verification (streaming to avoid loading entire file into memory)
            string hashHex;
            using (FileStream hashStream = File.OpenRead(patchZipPath))
            {
                byte[] hashBytes = await SHA256.HashDataAsync(hashStream, token).ConfigureAwait(false);
                hashHex = Convert.ToHexString(hashBytes);
            }

            if (!string.Equals(hashHex, packageInfo.PatchSha256, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(patchZipPath);
                return false;
            }

            // Extract to files directory
            try
            {
                ZipFile.ExtractToDirectory(patchZipPath, filesDir, true);
            }
            catch (Exception ex)
            {
                if (Directory.Exists(filesDir))
                {
                    Directory.Delete(filesDir, true);
                }

                File.Delete(patchZipPath);
                SentrySdk.CaptureException(ex);
                return false;
            }

            // Write version marker
            await File.WriteAllTextAsync(versionMarker, packageInfo.Version.ToString(), token).ConfigureAwait(false);

            // Clean up zip
            File.Delete(patchZipPath);

            return true;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            return false;
        }
        finally
        {
            s_patchDownloadLock.Release();
        }
    }

    private void ShowUpdateLogWindowAfterUpdate() { }

    private void NotifyIfDataFolderHasReparsePoint()
    {
        if (new DirectoryInfo(kyxsanRuntime.DataDirectory).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug("Data folder has reparse point", "MainViewModel.Command"));
            messenger.Send(InfoBarMessage.Warning(SH.FormatViewModelTitleDataFolderHasReparsepoint(kyxsanRuntime.DataDirectory)));
        }
    }
}