//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Web.Response;

namespace kyxsan.Web.kyxsan.Response;

internal sealed class kyxsanResponse : Web.Response.Response, ILocalizableResponse, ICommonResponse<kyxsanResponse>
{
    [JsonConstructor]
    public kyxsanResponse(int returnCode, string message, string? localizationKey)
        : base(returnCode, message)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    static kyxsanResponse ICommonResponse<kyxsanResponse>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class kyxsanResponse<TData> : Response<TData>, ILocalizableResponse, ICommonResponse<kyxsanResponse<TData>>
{
    [JsonConstructor]
    public kyxsanResponse(int returnCode, string message, TData? data, string? localizationKey)
        : base(returnCode, message, data)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    static kyxsanResponse<TData> ICommonResponse<kyxsanResponse<TData>>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}