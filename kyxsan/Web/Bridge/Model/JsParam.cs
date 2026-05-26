//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Web.Bridge.Model;

internal sealed class JsParam
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = default!;

    [JsonPropertyName("payload")]
    public JsonElement Payload { get; set; }

    [JsonPropertyName("callback")]
    public string? Callback { get; set; }
}

[SuppressMessage("", "SA1402")]
internal sealed class JsParam<TPayload>
{
    [JsonPropertyName("method")]
    public string Method { get; set; } = default!;

    [JsonPropertyName("payload")]
    public TPayload Payload { get; set; } = default!;

    [JsonPropertyName("callback")]
    public string? Callback { get; set; }

    public static implicit operator JsParam<TPayload>(JsParam jsParam)
    {
        return new()
        {
            Method = jsParam.Method,
            Payload = jsParam.Payload.Deserialize<TPayload>() ?? default!,
            Callback = jsParam.Callback,
        };
    }
}