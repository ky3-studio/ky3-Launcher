//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.kyxsan.Geetest;

internal sealed class GeetestResponse
{
    public static GeetestResponse InternalFailure { get; } = new() { Code = Web.Response.Response.InternalFailure };

    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("info")]
    public string Info { get; set; } = default!;

    [JsonPropertyName("data")]
    public GeetestData Data { get; set; } = default!;

    [JsonPropertyName("times")]
    public int Times { get; set; } = default!;
}