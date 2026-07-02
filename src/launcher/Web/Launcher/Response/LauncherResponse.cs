//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Web.Response;

namespace Launcher.Web.Launcher.Response;

internal sealed class LauncherResponse : Web.Response.Response, ILocalizableResponse, ICommonResponse<LauncherResponse>
{
    [JsonConstructor]
    public LauncherResponse(int returnCode, string message, string? localizationKey)
        : base(returnCode, message)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    static LauncherResponse ICommonResponse<LauncherResponse>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}

[SuppressMessage("", "SA1402")]
internal sealed class LauncherResponse<TData> : Response<TData>, ILocalizableResponse, ICommonResponse<LauncherResponse<TData>>
{
    [JsonConstructor]
    public LauncherResponse(int returnCode, string message, TData? data, string? localizationKey)
        : base(returnCode, message, data)
    {
        LocalizationKey = localizationKey;
    }

    [JsonPropertyName("l10nKey")]
    public string? LocalizationKey { get; set; }

    static LauncherResponse<TData> ICommonResponse<LauncherResponse<TData>>.CreateDefault(int returnCode, string message)
    {
        return new(returnCode, message, default, default);
    }

    public override string ToString()
    {
        return SH.FormatWebResponse(ReturnCode, this.GetLocalizationMessageOrDefault());
    }
}
