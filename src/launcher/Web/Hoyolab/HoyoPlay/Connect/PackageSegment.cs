//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.DataTransfer;
using Launcher.Core.Logging;
using Launcher.Service.Notification;

namespace Launcher.Web.Hoyolab.HoyoPlay.Connect;

internal partial class PackageSegment
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("md5")]
    public string MD5 { get; set; } = default!;

    [JsonPropertyName("size")]
    public long Size { get; set; } = default!;

    [JsonPropertyName("decompressed_size")]
    public long DecompressedSize { get; set; } = default!;

    [JsonIgnore]
    public string DisplayName { get => System.IO.Path.GetFileName(Url); }

    [Command("CopyPathCommand")]
    private async Task CopyPathToClipboardAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy url to ClipBoard", "PackageSegment.Command"));

        IServiceProvider serviceProvider = Ioc.Default;
        await serviceProvider.GetRequiredService<IClipboardProvider>().SetTextAsync(Url).ConfigureAwait(false);
        serviceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Success(SH.WebGameResourcePathCopySucceed));
    }
}
