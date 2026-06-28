//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Launcher.Core;
using Launcher.Core.ApplicationModel;
using Launcher.Core.LifeCycle;
using Launcher.Core.Logging;
using Launcher.Service;
using Launcher.Service.BackgroundActivity;
using Launcher.Service.Metadata;
using Launcher.Service.Notification;
using Launcher.Service.RedeemCode;
using Launcher.Service.RemoteConfig;
using Launcher.Core.IO.Http.Proxy;
using Launcher.Factory.ContentDialog;
using Launcher.Service.Update;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;

namespace Launcher.ViewModel;

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

    public static string? Title { get => LauncherRuntime.GetDisplayName(); }

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
        CleanUpdateCache();
        ShowUpdateLogWindowAfterUpdate();
        NotifyIfDataFolderHasReparsePoint();

        _ = RedeemCodeService.RefreshAsync();
        _ = CheckAndPrepareUpdateAsync();
        _ = StartPeriodicUpdateCheckAsync();

        return true;
    }

    private static void CleanUpdateCache()
    {
        try
        {
            string updateCacheDir = Path.Combine(LauncherRuntime.DataDirectory, "UpdateCache");
            if (Directory.Exists(updateCacheDir))
            {
                Directory.Delete(updateCacheDir, true);
            }
        }
        catch
        {
            // Best effort cleanup, ignore errors
        }
    }

    [Command("RestartToUpdateCommand")]
    private void RestartToUpdate()
    {
        try
        {
            string updateFilesDir = Path.Combine(LauncherRuntime.DataDirectory, "UpdateCache", "files");
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

            await taskContext.SwitchToMainThreadAsync();
            Microsoft.UI.Xaml.Controls.ContentDialog updatePromptDialog = new()
            {
                XamlRoot = currentXamlWindowReference.XamlRoot,
                Title = SH.ViewModelMainUpdateMandatoryTitle,
                Content = SH.FormatViewModelMainUpdateMandatoryContent(result.PackageInformation.Version.ToString()),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = SH.ViewModelMainUpdateNowButton,
                CloseButtonText = SH.ViewModelMainUpdateLaterButton,
                RequestedTheme = currentXamlWindowReference.RequestedTheme,
            };
            ContentDialogResult dialogResult = await contentDialogFactory.EnqueueAndShowAsync(updatePromptDialog).ShowTask.ConfigureAwait(false);

            if (string.IsNullOrEmpty(result.PackageInformation.PatchUrl) ||
                string.IsNullOrEmpty(result.PackageInformation.PatchSha256))
            {
                messenger.Send(InfoBarMessage.Warning(SH.ViewModelMainUpdatePatchNotAvailable));
                return;
            }

            if (dialogResult is ContentDialogResult.Primary)
            {
                ContentDialog progressDialog = await contentDialogFactory
                    .CreateForIndeterminateProgressAsync(SH.ViewModelMainUpdateDownloading)
                    .ConfigureAwait(false);

                await taskContext.SwitchToMainThreadAsync();
                ProgressBar progressBar = new() { Minimum = 0, Maximum = 100, Value = 0 };
                TextBlock statusText = new()
                {
                    Text = SH.ViewModelMainUpdateDownloading,
                    Margin = new Microsoft.UI.Xaml.Thickness(0, 8, 0, 0),
                    FontSize = 12,
                };
                progressDialog.Content = new StackPanel
                {
                    Spacing = 4,
                    Children = { progressBar, statusText },
                };

                Progress<double> progress = new(p =>
                {
                    progressBar.Value = p;
                    if (p >= 95)
                    {
                        statusText.Text = SH.ViewModelMainUpdateExtracting;
                    }
                    else if (p >= 90)
                    {
                        statusText.Text = SH.ViewModelMainUpdateVerifying;
                    }
                });

                bool patchSuccess;
                using (await contentDialogFactory.BlockAsync(progressDialog).ConfigureAwait(false))
                {
                    patchSuccess = await DownloadAndExtractPatchAsync(result.PackageInformation, progress).ConfigureAwait(false);
                }

                if (patchSuccess)
                {
                    await taskContext.SwitchToMainThreadAsync();
                    RestartToUpdate();
                }
                else
                {
                    await taskContext.SwitchToMainThreadAsync();
                    messenger.Send(InfoBarMessage.Warning(SH.ViewModelMainUpdateDownloadFailed));
                }
            }
            else
            {
                bool patchSuccess = await DownloadAndExtractPatchAsync(result.PackageInformation).ConfigureAwait(false);
                if (patchSuccess)
                {
                    await taskContext.SwitchToMainThreadAsync();
                    IsUpdateReady = true;
                    UpdateReadyNotifyToken++;
                }
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    internal static async Task<bool> DownloadAndExtractPatchAsync(Web.Launcher.LauncherPackageInformation packageInfo, IProgress<double>? progress = null, CancellationToken token = default)
    {
        await s_patchDownloadLock.WaitAsync(token).ConfigureAwait(false);
        try
        {
            string updateCacheDir = Path.Combine(LauncherRuntime.DataDirectory, "UpdateCache");
            string filesDir = Path.Combine(updateCacheDir, "files");
            string versionMarker = Path.Combine(updateCacheDir, "version.txt");

            if (Directory.Exists(filesDir) && File.Exists(versionMarker))
            {
                string cachedVersion = await File.ReadAllTextAsync(versionMarker, token).ConfigureAwait(false);
                if (cachedVersion.Trim() == packageInfo.Version.ToString())
                {
                    progress?.Report(100);
                    return true;
                }
            }

            if (Directory.Exists(filesDir))
            {
                Directory.Delete(filesDir, true);
            }

            Directory.CreateDirectory(updateCacheDir);

            string patchUrl = packageInfo.PatchUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? packageInfo.PatchUrl
                : $"{BackendApiRoutes.ServerRoot}{packageInfo.PatchUrl}";

            string patchZipPath = Path.Combine(updateCacheDir, "patch.zip");

            SocketsHttpHandler handler = new()
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true,
                },
                Proxy = HttpProxyUsingSystemProxy.Instance,
                UseProxy = true,
                PooledConnectionLifetime = TimeSpan.FromMinutes(10),
                ConnectTimeout = TimeSpan.FromSeconds(30),
            };

            using HttpClient client = new(handler) { Timeout = Timeout.InfiniteTimeSpan };

            const int maxRetries = 5;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    long existingBytes = 0;
                    if (File.Exists(patchZipPath))
                    {
                        existingBytes = new FileInfo(patchZipPath).Length;
                    }

                    using HttpRequestMessage request = new(HttpMethod.Get, patchUrl.ToUri());
                    if (existingBytes > 0)
                    {
                        request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(existingBytes, null);
                    }

                    using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);

                    long totalBytes;
                    long downloadedBytes;
                    FileMode fileMode;

                    if (response.StatusCode is System.Net.HttpStatusCode.PartialContent && existingBytes > 0)
                    {
                        totalBytes = existingBytes + (response.Content.Headers.ContentLength ?? 0);
                        downloadedBytes = existingBytes;
                        fileMode = FileMode.Append;
                    }
                    else
                    {
                        response.EnsureSuccessStatusCode();
                        totalBytes = response.Content.Headers.ContentLength ?? 0;
                        downloadedBytes = 0;
                        fileMode = FileMode.Create;
                    }

                    using (Stream netStream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false))
                    using (FileStream fileStream = new(patchZipPath, fileMode, FileAccess.Write, FileShare.None))
                    {
                        byte[] buffer = new byte[81920];
                        int bytesRead;
                        while ((bytesRead = await netStream.ReadAsync(buffer, token).ConfigureAwait(false)) > 0)
                        {
                            await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token).ConfigureAwait(false);
                            downloadedBytes += bytesRead;
                            if (totalBytes > 0)
                            {
                                progress?.Report((double)downloadedBytes / totalBytes * 90);
                            }
                        }
                    }

                    break;
                }
                catch (Exception) when (attempt < maxRetries - 1)
                {
                    await Task.Delay(2000 * (attempt + 1), token).ConfigureAwait(false);
                }
            }

            progress?.Report(92);

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

            progress?.Report(95);

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

            await File.WriteAllTextAsync(versionMarker, packageInfo.Version.ToString(), token).ConfigureAwait(false);
            File.Delete(patchZipPath);

            progress?.Report(100);
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
        if (new DirectoryInfo(LauncherRuntime.DataDirectory).Attributes.HasFlag(FileAttributes.ReparsePoint))
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug("Data folder has reparse point", "MainViewModel.Command"));
            messenger.Send(InfoBarMessage.Warning(SH.FormatViewModelTitleDataFolderHasReparsepoint(LauncherRuntime.DataDirectory)));
        }
    }
}