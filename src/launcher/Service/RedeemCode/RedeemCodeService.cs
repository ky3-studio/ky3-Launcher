//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.Core.DataTransfer;
using Launcher.Core.IO.Http;
using Launcher.Service.Constants;
using Launcher.Service.Notification;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Launcher.Service.RedeemCode;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class RedeemCodeService : ObservableObject
{
    private const string BgiCodesUrl = "https://cnb.cool/bettergi/genshin-redeem-code/-/git/raw/main/codes.json";
    private const string HoyoCodesUrl = "https://hoyo-codes.seria.moe/codes?game=genshin";

    private readonly IHttpClientFactory httpClientFactory;
    private readonly ITaskContext taskContext;
    private readonly IClipboardProvider clipboardProvider;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial RedeemCodeService(IServiceProvider serviceProvider);

    public ObservableCollection<RedeemCodeItem> CnRedeemCodes { get; } = [];

    public ObservableCollection<RedeemCodeItem> OsRedeemCodes { get; } = [];

    [ObservableProperty]
    private bool hasCnCodes;

    [ObservableProperty]
    private bool hasOsCodes;

    public ICommand CopyCodeCommand => new AsyncRelayCommand<string>(CopyCodeAsync);

    public async ValueTask RefreshAsync()
    {
        List<RedeemCodeItem> cnItems = [];
        List<RedeemCodeItem> osItems = [];

        await Task.WhenAll(
            FetchBgiCodesAsync(cnItems),
            FetchHoyoCodesAsync(osItems)).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();

        CnRedeemCodes.Clear();
        foreach (RedeemCodeItem item in cnItems)
        {
            CnRedeemCodes.Add(item);
        }

        OsRedeemCodes.Clear();
        foreach (RedeemCodeItem item in osItems)
        {
            OsRedeemCodes.Add(item);
        }

        HasCnCodes = CnRedeemCodes.Count > 0;
        HasOsCodes = OsRedeemCodes.Count > 0;
    }

    private async Task CopyCodeAsync(string? code)
    {
        if (!string.IsNullOrEmpty(code))
        {
            await clipboardProvider.SetTextAsync(code).ConfigureAwait(false);
            messenger.Send(InfoBarMessage.Success(SH.FormatUIViewMainRedeemCodeCopySucceed(code)));
        }
    }

    private async Task FetchBgiCodesAsync(List<RedeemCodeItem> items)
    {
        try
        {
            using HttpClient client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DownloadTimeoutSeconds);
            using HttpRequestMessage bgiRequest = new(HttpMethod.Get, BgiCodesUrl);
            bgiRequest.Options.Set(RetryHttpHandler.DisableRetry, true);
            using HttpResponseMessage bgiResponse = await client.SendAsync(bgiRequest).ConfigureAwait(false);
            bgiResponse.EnsureSuccessStatusCode();
            string json = await bgiResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            List<BgiRedeemCodeEntry>? entries = JsonSerializer.Deserialize<List<BgiRedeemCodeEntry>>(json);

            if (entries is null)
            {
                return;
            }

            lock (items)
            {
                foreach (BgiRedeemCodeEntry entry in entries)
                {
                    if (!DateOnly.TryParse(entry.Valid, out DateOnly validDate) || validDate < DateOnly.FromDateTime(DateTime.Now))
                    {
                        continue;
                    }

                    foreach (string code in entry.Codes)
                    {
                        items.Add(new RedeemCodeItem
                        {
                            Code = code,
                            Description = entry.Content,
                            ValidUntil = SH.FormatUIViewMainRedeemCodeValidUntil(entry.Valid),
                            Server = RedeemCodeServer.CN,
                        });
                    }
                }
            }
        }
        catch (Exception ex) when (ex is TaskCanceledException or HttpRequestException or OperationCanceledException or JsonException)
        {
            // 网络/DNS 异常或接口返回非 JSON（如 HTML 503 页面），第三方服务预期行为，静默处理
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    private async Task FetchHoyoCodesAsync(List<RedeemCodeItem> items)
    {
        try
        {
            using HttpClient client = httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(LauncherApiConstants.DownloadTimeoutSeconds);
            using HttpRequestMessage hoyoRequest = new(HttpMethod.Get, HoyoCodesUrl);
            hoyoRequest.Options.Set(RetryHttpHandler.DisableRetry, true);
            using HttpResponseMessage hoyoResponse = await client.SendAsync(hoyoRequest).ConfigureAwait(false);
            hoyoResponse.EnsureSuccessStatusCode();
            string json = await hoyoResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            HoyoCodesResponse? response = JsonSerializer.Deserialize<HoyoCodesResponse>(json);

            if (response?.Codes is null)
            {
                return;
            }

            lock (items)
            {
                foreach (HoyoCodeEntry entry in response.Codes)
                {
                    if (!string.Equals(entry.Status, "OK", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    items.Add(new RedeemCodeItem
                    {
                        Code = entry.Code,
                        Description = entry.Rewards?.Replace(";", ", ") ?? string.Empty,
                        ValidUntil = string.Empty,
                        Server = RedeemCodeServer.OS,
                    });
                }
            }
        }
        catch (Exception ex) when (ex is TaskCanceledException or HttpRequestException or OperationCanceledException or JsonException)
        {
            // 网络/DNS 异常或接口返回非 JSON（如 HTML 503 页面），第三方服务预期行为，静默处理
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
}
