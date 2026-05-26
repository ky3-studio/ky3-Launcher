//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.Web.WebView2.Core;
using kyxsan.Core.DataTransfer;
using kyxsan.Factory.Picker;
using System.Net.Http;

namespace kyxsan.Web.Bridge;

internal sealed class BridgeShareContext
{
    public required CoreWebView2 CoreWebView2 { get; init; }

    public required ITaskContext TaskContext { get; init; }

    public required HttpClient HttpClient { get; init; }

    public required IClipboardProvider ClipboardProvider { get; init; }

    public required JsonSerializerOptions JsonSerializerOptions { get; init; }

    public required IFileSystemPickerInteraction FileSystemPickerInteraction { get; init; }

    public required BridgeShareSaveType ShareSaveType { get; init; }

    public required IMessenger Messenger { get; init; }
}