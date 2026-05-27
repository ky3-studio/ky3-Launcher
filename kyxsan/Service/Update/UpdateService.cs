//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.Common;
using CommunityToolkit.Mvvm.Input;
using kyxsan.Core;
using kyxsan.Core.IO.Http.Proxy;
using kyxsan.Core.Setting;
using kyxsan.Service.Notification;
using kyxsan.Web.kyxsan;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Response;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;

namespace kyxsan.Service.Update;

[Service(ServiceLifetime.Singleton, typeof(IUpdateService))]
internal sealed partial class UpdateService : IUpdateService
{
    // Avoid injecting services directly
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial UpdateService(IServiceProvider serviceProvider);

    public string? UpdateInfo { get; set; }

    public async ValueTask<CheckUpdateResult> CheckUpdateAsync(CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CheckUpdateResult checkUpdateResult = new();
            try
            {
                ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
                await taskContext.SwitchToBackgroundAsync();

                kyxsanInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<kyxsanInfrastructureClient>();
                kyxsanResponse<kyxsanPackageInformation> response = await infrastructureClient.GetkyxsanVersionInformationAsync(token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidateWithoutUINotification(response, scope.ServiceProvider, out kyxsanPackageInformation? packageInformation))
                {
                    checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                    return checkUpdateResult;
                }

                checkUpdateResult.Kind = CheckUpdateResultKind.UpdateAvailable;
                checkUpdateResult.PackageInformation = packageInformation;

                if (!LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false))
                {
                    // Launched in an updated version
                    if (kyxsanRuntime.Version >= checkUpdateResult.PackageInformation.Version)
                    {
                        checkUpdateResult.Kind = CheckUpdateResultKind.AlreadyUpdated;
                        return checkUpdateResult;
                    }
                }

                if (checkUpdateResult.PackageInformation.Validation is not { Length: > 0 })
                {
                    checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidSha256;
                }

                return checkUpdateResult;
            }
            finally
            {
                UpdateInfo = checkUpdateResult.Kind switch
                {
                    CheckUpdateResultKind.UpdateAvailable => SH.FormatViewModelSettingUpdateAvailable(checkUpdateResult.PackageInformation?.Version.ToString()),
                    CheckUpdateResultKind.AlreadyUpdated => SH.ViewModelSettingAlreadyUpdated,
                    CheckUpdateResultKind.VersionApiInvalidResponse or CheckUpdateResultKind.VersionApiInvalidSha256 => SH.ViewModelSettingCheckUpdateFailed,
                    _ => default,
                };
            }
        }
    }

    public async ValueTask<string?> DownloadUpdateAsync(CheckUpdateResult result, Action<long, long>? onProgress = null, CancellationToken token = default)
    {
        if (result.Kind is not CheckUpdateResultKind.UpdateAvailable || result.PackageInformation is null)
        {
            return null;
        }

        if (result.PackageInformation.Mirrors is not { Count: > 0 } mirrors)
        {
            return null;
        }

        string tempPath = Path.Combine(Path.GetTempPath(), $"ky3launcher_update_v{result.PackageInformation.Version}.exe");

        // If already downloaded and verified, return directly
        if (File.Exists(tempPath))
        {
            byte[] existingHash = SHA256.HashData(await File.ReadAllBytesAsync(tempPath, token).ConfigureAwait(false));
            if (string.Equals(Convert.ToHexString(existingHash), result.PackageInformation.Validation, StringComparison.OrdinalIgnoreCase))
            {
                onProgress?.Invoke(1, 1);
                return tempPath;
            }

            File.Delete(tempPath);
        }

        // Try each mirror with retry
        foreach (kyxsanPackageMirror mirror in mirrors)
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    using HttpClient downloadClient = CreateDownloadHttpClient();
                    using HttpResponseMessage response = await downloadClient.GetAsync(mirror.Url.ToUri(), HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    long totalBytes = response.Content.Headers.ContentLength ?? -1;
                    long bytesDownloaded = 0;

                    using Stream netStream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);
                    using FileStream fileStream = File.Create(tempPath);

                    byte[] buffer = new byte[81920];
                    int bytesRead;
                    while ((bytesRead = await netStream.ReadAsync(buffer, token).ConfigureAwait(false)) > 0)
                    {
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), token).ConfigureAwait(false);
                        bytesDownloaded += bytesRead;
                        onProgress?.Invoke(bytesDownloaded, totalBytes);
                    }

                    await fileStream.FlushAsync(token).ConfigureAwait(false);

                    // SHA256 verification
                    byte[] fileBytes = await File.ReadAllBytesAsync(tempPath, token).ConfigureAwait(false);
                    byte[] hash = SHA256.HashData(fileBytes);
                    string hashHex = Convert.ToHexString(hash);

                    if (string.Equals(hashHex, result.PackageInformation.Validation, StringComparison.OrdinalIgnoreCase))
                    {
                        return tempPath;
                    }

                    // Hash mismatch, try next mirror
                    try { File.Delete(tempPath); } catch { }
                    break;
                }
                catch (OperationCanceledException)
                {
                    try { File.Delete(tempPath); } catch { }
                    return null;
                }
                catch
                {
                    try { File.Delete(tempPath); } catch { }
                    if (attempt < 1)
                    {
                        await Task.Delay(1000, token).ConfigureAwait(false);
                    }
                }
            }
        }

        return null;
    }

    private static HttpClient CreateDownloadHttpClient()
    {
        string? proxyUrl = HttpProxyUsingSystemProxy.Instance.CurrentProxyUri;
        SocketsHttpHandler handler = new()
        {
            ConnectTimeout = TimeSpan.FromSeconds(15),
            SslOptions = new System.Net.Security.SslClientAuthenticationOptions
            {
                RemoteCertificateValidationCallback = (_, _, _, _) => true,
            },
        };

        if (!string.IsNullOrEmpty(proxyUrl))
        {
            handler.Proxy = new WebProxy(proxyUrl);
            handler.UseProxy = true;
        }

        return new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(10) };
    }

    public async ValueTask TriggerUpdateAsync(CheckUpdateResult result, CancellationToken token = default)
    {
        if (result.Kind is not CheckUpdateResultKind.UpdateAvailable)
        {
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IMessenger messenger = scope.ServiceProvider.GetRequiredService<IMessenger>();
            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            DispatcherQueue dispatcherQueue = taskContext.DispatcherQueue;

            try
            {
                // Create progress UI elements on the UI thread
                ProgressBar progressBar = null!;
                TextBlock progressText = null!;
                StackPanel progressPanel = null!;

                dispatcherQueue.TryEnqueue(() =>
                {
                    progressBar = new ProgressBar
                    {
                        Minimum = 0,
                        Maximum = 100,
                        Value = 0,
                        Height = 4,
                        Margin = new Thickness(0, 8, 0, 0),
                    };
                    progressText = new TextBlock
                    {
                        Text = "准备下载...",
                        FontSize = 12,
                        Margin = new Thickness(0, 4, 0, 0),
                        Opacity = 0.8,
                    };
                    progressPanel = new StackPanel
                    {
                        Children = { progressBar, progressText },
                    };
                });

                // Small delay to ensure UI elements are created
                await Task.Delay(50, token).ConfigureAwait(false);

                messenger.Send(new InfoBarMessage
                {
                    Severity = InfoBarSeverity.Informational,
                    Title = $"正在下载新版本 v{result.PackageInformation?.Version}",
                    Content = progressPanel,
                    MilliSecondsDelay = 0,
                });

                long lastReportTime = Stopwatch.GetTimestamp();
                string? installerPath = await DownloadUpdateAsync(result, (bytesDownloaded, totalBytes) =>
                {
                    // Throttle UI updates to avoid flooding the dispatcher
                    long now = Stopwatch.GetTimestamp();
                    if (Stopwatch.GetElapsedTime(lastReportTime, now).TotalMilliseconds < 100)
                    {
                        return;
                    }

                    lastReportTime = now;
                    dispatcherQueue.TryEnqueue(() =>
                    {
                        if (totalBytes > 0)
                        {
                            double percent = (double)bytesDownloaded / totalBytes * 100;
                            progressBar.Value = percent;
                            progressBar.IsIndeterminate = false;
                            progressText.Text = $"{Converters.ToFileSizeString(bytesDownloaded)} / {Converters.ToFileSizeString(totalBytes)}  ({percent:F1}%)";
                        }
                        else
                        {
                            progressBar.IsIndeterminate = true;
                            progressText.Text = $"已下载 {Converters.ToFileSizeString(bytesDownloaded)}";
                        }
                    });
                }, token).ConfigureAwait(false);

                if (installerPath is null)
                {
                    messenger.Send(InfoBarMessage.Error("下载更新失败", "安装包下载或校验失败，请稍后重试"));
                    return;
                }

                IMessenger messengerForButton = messenger;
                messenger.Send(new InfoBarMessage
                {
                    Severity = InfoBarSeverity.Success,
                    Title = $"新版本 v{result.PackageInformation?.Version} 已准备就绪",
                    Message = "安装包已下载完成，点击「立即更新」重启并安装",
                    ActionButtonContent = "立即更新",
                    ActionButtonCommand = new AsyncRelayCommand(async () =>
                    {
                        try
                        {
                            if (!File.Exists(installerPath))
                            {
                                messengerForButton.Send(InfoBarMessage.Error("更新失败", "安装包文件已丢失，请重启软件重新下载"));
                                return;
                            }

                            // Show installing message before exit
                            messengerForButton.Send(new InfoBarMessage
                            {
                                Severity = InfoBarSeverity.Informational,
                                Title = $"正在安装 v{result.PackageInformation?.Version}",
                                Message = "安装完成后将自动启动新版本，请稍候...",
                                MilliSecondsDelay = 0,
                            });

                            // Brief delay so user sees the message
                            await Task.Delay(1000).ConfigureAwait(true);

                            // Inno Setup silent install; installer's [Run] entry will relaunch the app afterwards
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = installerPath,
                                Arguments = "/VERYSILENT /SUPPRESSMSGBOXES /CLOSEAPPLICATIONS /NORESTART",
                                UseShellExecute = true,
                                Verb = "runas",
                            });

                            Application.Current.Exit();
                        }
                        catch (Exception ex)
                        {
                            messengerForButton.Send(InfoBarMessage.Error("启动安装器失败", ex.Message));
                        }
                    }),
                    MilliSecondsDelay = 0,
                });
            }
            catch (Exception ex)
            {
                messenger.Send(InfoBarMessage.Error(ex));
            }
        }
    }
}