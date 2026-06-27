//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Launcher.Core.DataTransfer;
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
            string json = await client.GetStringAsync(BgiCodesUrl).ConfigureAwait(false);
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
            string json = await client.GetStringAsync(HoyoCodesUrl).ConfigureAwait(false);
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
                        Description = string.Empty,
                        ValidUntil = string.Empty,
                        Server = RedeemCodeServer.OS,
                    });
                }
            }
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }
}
