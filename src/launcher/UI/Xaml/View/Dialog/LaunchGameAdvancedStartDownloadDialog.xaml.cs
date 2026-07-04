//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Common;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SharpCompress.Archives.SevenZip;
using Launcher.Core;
using Launcher.Core.ExceptionService;
using Launcher.Core.IO;
using Launcher.Core.Setting;
using Launcher.Factory.ContentDialog;
using Launcher.Factory.Progress;
using Launcher.Service.Notification;
using Launcher.Web.Request.Builder;
using Launcher.Web.Request.Builder.Abstraction;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;

namespace Launcher.UI.Xaml.View.Dialog;

[DependencyProperty<ObservableCollection<AdvancedStartProgramOption>>("Programs")]
[DependencyProperty<AdvancedStartProgramOption>("SelectedProgram", PropertyChangedCallbackName = nameof(OnSelectedProgramChanged))]
[DependencyProperty<double>("ProgressValue", DefaultValue = 0d, NotNull = true)]
[DependencyProperty<string>("StatusText")]
[DependencyProperty<bool>("IsBusy", DefaultValue = false, NotNull = true, PropertyChangedCallbackName = nameof(OnIsBusyChanged))]
[DependencyProperty<bool>("IsLoading", DefaultValue = false, NotNull = true, PropertyChangedCallbackName = nameof(OnIsLoadingChanged))]
[DependencyProperty<string>("ErrorText")]
[DependencyProperty<bool>("HasError", DefaultValue = false, NotNull = true)]
[DependencyProperty<bool>("CanDownload", DefaultValue = false, NotNull = true)]
[DependencyProperty<bool>("IsListEnabled", DefaultValue = true, NotNull = true)]
internal sealed partial class LaunchGameAdvancedStartDownloadDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IHttpRequestMessageBuilderFactory httpRequestMessageBuilderFactory;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;
    private readonly JsonSerializerOptions jsonOptions;
    private readonly HttpClient httpClient;
    private IProgress<StreamCopyStatus>? downloadProgress;

    private string? downloadedPath;
    private string currentProgramName = string.Empty;

    [GeneratedConstructor(InitializeComponent = true)]
    public partial LaunchGameAdvancedStartDownloadDialog(IServiceProvider serviceProvider);

    partial void PostConstruct(IServiceProvider serviceProvider)
    {
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(LauncherRuntime.UserAgent);
        downloadProgress = serviceProvider.GetRequiredService<IProgressFactory>().CreateForMainThread<StreamCopyStatus>(UpdateDownloadProgress);
        Programs = [];
    }

    public async ValueTask<ValueResult<bool, string?>> GetResultAsync()
    {
        ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
        return new(result is ContentDialogResult.Primary && downloadedPath is not null, downloadedPath);
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        _ = LoadProgramsAsync();
    }

    private async Task LoadProgramsAsync()
    {
        await taskContext.SwitchToMainThreadAsync();
        IsLoading = true;
        HasError = false;
        ErrorText = string.Empty;
        StatusText = SH.ViewDialogLaunchGameAdvancedStartLoading;
        UpdateCanDownload();

        string feedEndpoint = LocalSetting.Get(SettingKeys.LaunchAdvancedStartFeedEndpoint, "https://Launcherfeed.pages.dev/programs.json");
        if (string.IsNullOrWhiteSpace(feedEndpoint))
        {
            HasError = true;
            ErrorText = SH.ViewDialogLaunchGameAdvancedStartFeedMissing;
            IsLoading = false;
            UpdateCanDownload();
            return;
        }

        try
        {
            await taskContext.SwitchToBackgroundAsync();
            ObservableCollection<AdvancedStartProgramOption> programs = await FetchProgramsAsync(feedEndpoint).ConfigureAwait(false);

            await taskContext.SwitchToMainThreadAsync();
            Programs = programs;
            StatusText = SH.ViewDialogLaunchGameAdvancedStartPick;
        }
        catch (Exception ex)
        {
            await taskContext.SwitchToMainThreadAsync();
            HasError = true;
            ErrorText = SH.ViewDialogLaunchGameAdvancedStartLoadFailed;
            messenger.Send(InfoBarMessage.Error(ex));
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            IsLoading = false;
            UpdateCanDownload();
        }
    }

    private async Task<ObservableCollection<AdvancedStartProgramOption>> FetchProgramsAsync(string endpoint)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create().SetRequestUri(endpoint).Get();
        using HttpRequestMessage message = builder.HttpRequestMessage;
        using HttpResponseMessage response = await httpClient.SendAsync(message).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        await using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        if (await JsonSerializer.DeserializeAsync<AdvancedStartProgramFeed>(stream, jsonOptions).ConfigureAwait(false) is AdvancedStartProgramFeed { Programs: { Count: > 0 } programs })
        {
            return new(programs);
        }

        stream.Seek(0, SeekOrigin.Begin);
        if (await JsonSerializer.DeserializeAsync<AdvancedStartProgramFeed>(stream, jsonOptions).ConfigureAwait(false) is AdvancedStartProgramFeed { Items: { Count: > 0 } items })
        {
            return new(items);
        }

        stream.Seek(0, SeekOrigin.Begin);
        if (await JsonSerializer.DeserializeAsync<List<AdvancedStartProgramOption>>(stream, jsonOptions).ConfigureAwait(false) is { Count: > 0 } list)
        {
            return new(list);
        }

        return [];
    }

    private async void OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        args.Cancel = true;
        ContentDialogButtonClickDeferral deferral = args.GetDeferral();

        bool success = false;
        try
        {
            success = await DownloadAndExtractAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Ensure reporting runs on UI thread
            await taskContext.SwitchToMainThreadAsync();
            messenger.Send(InfoBarMessage.Error(ex));
            success = false;
        }

        // Must switch to UI thread before touching XAML/COM objects
        await taskContext.SwitchToMainThreadAsync();
        args.Cancel = !success;

        deferral.Complete();
    }

    private async Task<bool> DownloadAndExtractAsync()
    {
        AdvancedStartProgramOption program;

        // 鈶?UI 绾跨▼锛氬彧璇讳竴娆?DependencyProperty
        await taskContext.SwitchToMainThreadAsync();

        if (SelectedProgram is null)
        {
            messenger.Send(InfoBarMessage.Warning(
                SH.ViewDialogLaunchGameAdvancedStartPickWarning));
            return false;
        }

        program = SelectedProgram;              // 鈫?蹇収
        currentProgramName = program.Name;

        HasError = false;
        ErrorText = string.Empty;
        IsBusy = true;
        ProgressValue = 0;
        UpdateCanDownload();

        try
        {
            // 鈶?鍚庡彴绾跨▼锛氬彧浣跨敤鏅€氬璞?
            await taskContext.SwitchToBackgroundAsync();
            string programPath = await DownloadSelectedProgramAsync(program);

            // 鈶?UI 绾跨▼锛氭洿鏂?UI
            await taskContext.SwitchToMainThreadAsync();
            downloadedPath = programPath;
            StatusText = SH.FormatViewDialogLaunchGameAdvancedStartCompleted(programPath);
            ProgressValue = 1;
            return true;
        }
        catch (Exception ex)
        {
            await taskContext.SwitchToMainThreadAsync();
            HasError = true;
            ErrorText = SH.ViewDialogLaunchGameAdvancedStartFailed;
            messenger.Send(InfoBarMessage.Error(ex));
            return false;
        }
        finally
        {
            await taskContext.SwitchToMainThreadAsync();
            IsBusy = false;
            UpdateCanDownload();
        }
    }


    private async ValueTask<string> DownloadSelectedProgramAsync(AdvancedStartProgramOption program)
    {
        HttpRequestMessageBuilder builder = httpRequestMessageBuilderFactory.Create().SetRequestUri(program.Url).Get();

        using HttpRequestMessage message = builder.HttpRequestMessage;
        HttpResponseMessage response;
        try
        {
            response = await httpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);
        }
        catch (HttpRequestException ex) when (ex.InnerException is SocketException socketEx)
        {
            // 鐗瑰埆澶勭悊SocketException锛圖NS銆佽繛鎺ュけ璐ョ瓑锛?
            switch (socketEx.SocketErrorCode)
            {
                case SocketError.HostNotFound:  // 11001 - DNS瑙ｆ瀽澶辫触
                                                // 涓撻棬澶勭悊DNS瑙ｆ瀽澶辫触锛屾彁渚涙洿璇︾粏鐨勭敤鎴锋寚瀵?
                    string hostName = message.RequestUri?.Host ?? SH.ViewDialogLaunchGameAdvancedStartUnknownServer;
                    throw LauncherException.InvalidOperation(
                        string.Format(SH.ViewDialogLaunchGameAdvancedStartDnsFailed, hostName));

                case SocketError.ConnectionRefused:  // 10061 - 杩炴帴琚嫆缁?
                    throw LauncherException.InvalidOperation(SH.ViewDialogLaunchGameAdvancedStartConnectionRefused);

                case SocketError.TimedOut:  // 10060 - 杩炴帴瓒呮椂
                    throw LauncherException.InvalidOperation(SH.ViewDialogLaunchGameAdvancedStartTimedOut);

                case SocketError.NetworkDown:  // 10050 - 缃戠粶涓嶅彲鐢?
                    throw LauncherException.InvalidOperation(SH.ViewDialogLaunchGameAdvancedStartNetworkDown);

                case SocketError.NetworkUnreachable:  // 10051 - 缃戠粶涓嶅彲杈?
                    throw LauncherException.InvalidOperation(SH.ViewDialogLaunchGameAdvancedStartNetworkUnreachable);

                case SocketError.NoData:  // 10054 - DNS鏌ヨ鎴愬姛浣嗘棤璁板綍
                    throw LauncherException.InvalidOperation(SH.ViewDialogLaunchGameAdvancedStartDnsNoData);

                default:
                    // 璁板綍鍘熷寮傚父淇℃伅浠ヤ究璋冭瘯
                    throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartNetworkError, socketEx.Message, socketEx.SocketErrorCode));
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
        {
            // 澶勭悊鏈塇TTP鐘舵€佺爜鐨勫紓甯革紙濡?xx/5xx鍝嶅簲锛屼絾浠嶇劧鎶涘嚭寮傚父鐨勬儏鍐碉級
            // 娉ㄦ剰锛欻ttpRequestException.StatusCode鍦?NET 5+涓墠鍙敤
            if (ex.StatusCode is HttpStatusCode statusCode)
            {
                string statusCodeDescription = statusCode.ToString();

                throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartHttpError, (int)statusCode, statusCodeDescription));
            }
            else
            {
                throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartUnknownServerResponse, ex.Message));
            }
        }
        catch (HttpRequestException ex)
        {
            // 鍏朵粬HttpRequestException锛堝SSL/TLS閿欒绛夛級
            // 妫€鏌ユ槸鍚︽槸璇佷功鐩稿叧閿欒
            if (ex.Message.Contains("certificate", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("SSL", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("TLS", StringComparison.OrdinalIgnoreCase))
            {
                throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartSecurityFailed, ex.Message));
            }

            // 閫氱敤缃戠粶璇锋眰寮傚父
            throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartNetworkRequestFailed, ex.Message));
        }
        catch (TaskCanceledException ex)
        {
            // 鍖哄垎瓒呮椂鍜岀敤鎴峰彇娑?
            // TaskCanceledException鍙兘鐢变互涓嬪師鍥犲紩璧凤細
            // 1. 鐢ㄦ埛鍙栨秷鎿嶄綔 (CancellationToken.IsCancellationRequested == true)
            // 2. HttpClient瓒呮椂 (HttpClient.Timeout)
            // 3. 缃戠粶瓒呮椂

            if (ex.CancellationToken.IsCancellationRequested)
            {
                // 鐢ㄦ埛涓诲姩鍙栨秷
                throw LauncherException.OperationCanceled(string.Format(SH.ViewDialogLaunchGameAdvancedStartDownloadCanceled, program.Name));
            }
            else
            {
                // 閫氬父鏄敱HttpClient.Timeout寮曡捣鐨勮秴鏃?
                // 浣嗕篃鍙兘鏄綉缁滃眰鐨勮秴鏃?
                throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartDownloadTimeout, program.Name));
            }
        }
        catch (OperationCanceledException ex)
        {
            // 澶勭悊鏇撮€氱敤鐨勫彇娑堝紓甯革紙鏉ヨ嚜CancellationTokenSource锛?
            throw LauncherException.OperationCanceled(string.Format(SH.ViewDialogLaunchGameAdvancedStartOperationCanceled, ex.Message));
        }
        catch (IOException ex)
        {
            // 澶勭悊IO鐩稿叧鐨勫紓甯革紙濡傚啓鍏ユ枃浠舵椂锛?
            throw LauncherException.IO(string.Format(SH.ViewDialogLaunchGameAdvancedStartIOError, ex.Message));
        }
        catch (NotSupportedException ex) when (ex.Message.Contains("Content", StringComparison.OrdinalIgnoreCase))
        {
            // 澶勭悊涓嶆敮鎸佺殑鍐呭绫诲瀷
            throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartUnsupportedContentType, ex.Message));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("headers", StringComparison.OrdinalIgnoreCase))
        {
            // 澶勭悊鏃犳晥鐨凥TTP澶?
            throw LauncherException.InvalidOperation(string.Format(SH.ViewDialogLaunchGameAdvancedStartInvalidHeaders, ex.Message));
        }
        catch (Exception ex)
        {
            // 鍏朵粬鏈鏈熺殑寮傚父
            // 鎻愪緵鐢ㄦ埛鍙嬪ソ鐨勯敊璇俊鎭紝鍚屾椂淇濈暀鎶€鏈粏鑺備緵鎶€鏈敮鎸?
            throw LauncherException.InvalidOperation(
                string.Format(SH.ViewDialogLaunchGameAdvancedStartUnknownError, program.Name, ex.GetType().Name));
        }
        finally
        {
            // 娓呯悊璧勬簮
            // 娉ㄦ剰锛欻ttpRequestMessage閫氬父鐢辫皟鐢ㄦ柟閲婃斁锛屼絾鍦ㄨ繖閲屼綔涓哄眬閮ㄥ彉閲忥紝搴旇閲婃斁
            // 濡傛灉response琚甯歌繑鍥烇紝璋冪敤鏂硅礋璐ｉ噴鏀緍esponse
            message?.Dispose();
        }
        using (response)
        {
            response.EnsureSuccessStatusCode();

            long contentLength = response.Content.Headers.ContentLength ?? 0;
            await using Stream content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            await using TempFileStream tempStream = new(FileMode.OpenOrCreate, FileAccess.ReadWrite);

            using (StreamCopyWorker worker = new(content, tempStream, contentLength))
            {
                await worker.CopyAsync(downloadProgress!).ConfigureAwait(false);
            }

            tempStream.Seek(0, SeekOrigin.Begin);

            string baseFolder = Path.Combine(LauncherRuntime.DataDirectory, "AdvancedStart");
            Directory.CreateDirectory(baseFolder);

            string targetFolder = Path.Combine(baseFolder, SanitizePathSegment(program.Name), SanitizePathSegment(string.IsNullOrWhiteSpace(program.Version) ? "latest" : program.Version));

            if (Directory.Exists(targetFolder))
            {
                Directory.Delete(targetFolder, true);
            }

            Directory.CreateDirectory(targetFolder);
            await ExtractArchiveAsync(tempStream, targetFolder).ConfigureAwait(false);

            string? executablePath = ResolveExecutablePath(targetFolder, program.Entry);
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                throw LauncherException.InvalidOperation(SH.ViewDialogLaunchGameAdvancedStartExecutableMissing);
            }

            return executablePath;
        }
    }

    private async ValueTask ExtractArchiveAsync(Stream stream, string targetFolder)
    {
        // Detect archive type by signature
        byte[] header = new byte[6];
        int read = await stream.ReadAsync(header.AsMemory(0, header.Length)).ConfigureAwait(false);
        stream.Seek(0, SeekOrigin.Begin);

        bool isZip = read >= 2 && header[0] == 0x50 && header[1] == 0x4B; // 'PK'
        bool is7z = read >= 6 && header[0] == 0x37 && header[1] == 0x7A && header[2] == 0xBC && header[3] == 0xAF && header[4] == 0x27 && header[5] == 0x1C; // 7z signature

        if (isZip)
        {
            using ZipArchive archive = await ZipArchive.CreateAsync(stream, ZipArchiveMode.Read, false, default);
            int totalEntries = archive.Entries.Count;
            int current = 0;

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string destinationPath = Path.GetFullPath(Path.Combine(targetFolder, entry.FullName));
                if (!destinationPath.StartsWith(Path.GetFullPath(targetFolder), StringComparison.OrdinalIgnoreCase))
                {
                    current++;
                    continue;
                }

                string? destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                if (string.IsNullOrEmpty(entry.Name))
                {
                    current++;
                    continue;
                }

                await using FileStream fileStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await using Stream entryStream = entry.Open();
                await entryStream.CopyToAsync(fileStream).ConfigureAwait(false);

                current++;
                await taskContext.SwitchToMainThreadAsync();
                ProgressValue = totalEntries is 0 ? 0 : (double)current / totalEntries;
                StatusText = SH.FormatViewDialogLaunchGameAdvancedStartExtracting(currentProgramName, current, totalEntries);
            }

            return;
        }

        if (is7z)
        {
            // SharpCompress used to extract 7z archives.
            // Ensure the project references SharpCompress NuGet package.
            string? tempFilePath = null;
            Stream sevenZipStream = stream;
            try
            {
                // Ensure we provide a seekable stream to SharpCompress.
                if (!stream.CanSeek)
                {
                    // Copy to a temp file and reopen for reading.
                    tempFilePath = Path.GetTempFileName();
                    await using (FileStream fsTempWrite = File.Open(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        await stream.CopyToAsync(fsTempWrite).ConfigureAwait(false);
                        await fsTempWrite.FlushAsync().ConfigureAwait(false);
                    }

                    sevenZipStream = File.OpenRead(tempFilePath);
                }
                else
                {
                    sevenZipStream.Seek(0, SeekOrigin.Begin);
                }

                using SevenZipArchive archive = SevenZipArchive.Open(sevenZipStream);
                List<SevenZipArchiveEntry> entries = archive.Entries.Where(e => !e.IsDirectory).ToList();
                int totalEntries = entries.Count;
                int current = 0;

                foreach (SevenZipArchiveEntry entry in entries)
                {
                    string entryPath = (entry.Key ?? string.Empty).Replace('/', Path.DirectorySeparatorChar);
                    string destinationPath = Path.Combine(targetFolder, entryPath);
                    string? destinationDirectory = Path.GetDirectoryName(destinationPath);
                    if (!string.IsNullOrEmpty(destinationDirectory))
                    {
                        Directory.CreateDirectory(destinationDirectory);
                    }

                    try
                    {
                        await using Stream entryStream = entry.OpenEntryStream();
                        await using FileStream fileStream = File.Open(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                        await entryStream.CopyToAsync(fileStream).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType().Name == "DataErrorException" || ex.InnerException?.GetType().Name == "DataErrorException")
                        {
                            throw new InvalidDataException(string.Format(SH.ViewDialogLaunchGameAdvancedStart7zEntryDataError, entry.Key), ex);
                        }

                        throw;
                    }

                    current++;
                    await taskContext.SwitchToMainThreadAsync();
                    ProgressValue = totalEntries is 0 ? 0 : (double)current / totalEntries;
                    StatusText = SH.FormatViewDialogLaunchGameAdvancedStartExtracting(currentProgramName, current, totalEntries);
                }

                return;
            }
            catch (Exception ex)
            {
                // If underlying exception is the inaccessible LZMA DataErrorException, wrap and surface as InvalidDataException
                if (ex.GetType().Name == "DataErrorException" || ex.InnerException?.GetType().Name == "DataErrorException")
                {
                    throw new InvalidDataException(SH.ViewDialogLaunchGameAdvancedStart7zDataError, ex);
                }

                throw;
            }
            finally
            {
                try
                {
                    if (!ReferenceEquals(sevenZipStream, stream))
                    {
                        sevenZipStream.Dispose();
                    }
                }
                catch (Exception) { }

                if (tempFilePath is not null)
                {
                    FileOperationSafe.TryDelete(tempFilePath);
                }
            }
        }

        // Unsupported archive format
        throw new InvalidDataException(SH.ViewDialogLaunchGameAdvancedStartUnsupportedArchiveFormat);
    }

    private string? ResolveExecutablePath(string targetFolder, string? entryPath)
    {
        if (!string.IsNullOrWhiteSpace(entryPath))
        {
            string candidate = Path.Combine(targetFolder, entryPath);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        string[] executables = Directory.GetFiles(targetFolder, "*.exe", SearchOption.AllDirectories);
        return executables.FirstOrDefault();
    }

    private void UpdateDownloadProgress(StreamCopyStatus status)
    {
        StatusText = SH.FormatViewDialogLaunchGameAdvancedStartDownloading(currentProgramName, Converters.ToFileSizeString(status.BytesReadSinceCopyStart), Converters.ToFileSizeString(status.TotalBytes));
        ProgressValue = status.TotalBytes is 0 ? 0 : (double)status.BytesReadSinceCopyStart / status.TotalBytes;
    }

    private static void OnSelectedProgramChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((LaunchGameAdvancedStartDownloadDialog)sender).UpdateCanDownload();
    }

    private static void OnIsBusyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((LaunchGameAdvancedStartDownloadDialog)sender).UpdateCanDownload();
    }

    private static void OnIsLoadingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((LaunchGameAdvancedStartDownloadDialog)sender).UpdateCanDownload();
    }

    private void UpdateCanDownload()
    {
        IsListEnabled = !IsBusy && !IsLoading;
        CanDownload = SelectedProgram is not null && IsListEnabled && !HasError;
        IsPrimaryButtonEnabled = CanDownload;
    }

    private static string SanitizePathSegment(string segment)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            segment = segment.Replace(invalid, '_');
        }

        return segment;
    }

    private sealed class AdvancedStartProgramFeed
    {
        public List<AdvancedStartProgramOption> Programs { get; set; } = [];

        public List<AdvancedStartProgramOption>? Items { get; set; }
    }

    private async void ViewDialogLaunchGameAdvancedStartInfoBarButton_Click(object sender, RoutedEventArgs e)
    {
        string baseFolder = Path.Combine(LauncherRuntime.DataDirectory, "AdvancedStart");
        if (Directory.Exists(baseFolder))
        {
            await Windows.System.Launcher.LaunchFolderPathAsync(baseFolder);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning("The folder doesn't exist"));
        }
    }
}

internal sealed class AdvancedStartProgramOption
{
    public string Name { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public string? Entry { get; set; }

    public string? Description { get; set; }
}