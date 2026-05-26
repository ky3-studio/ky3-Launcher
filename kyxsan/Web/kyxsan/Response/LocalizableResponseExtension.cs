//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Globalization;

namespace kyxsan.Web.kyxsan.Response;

internal static class LocalizableResponseExtension
{
    extension(ILocalizableResponse localizableResponse)
    {
        public string? GetLocalizationMessageOrDefault()
        {
            string? key = localizableResponse.LocalizationKey;
            return string.IsNullOrEmpty(key) ? default : SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture);
        }

        public string GetLocalizationMessage()
        {
            string? key = localizableResponse.LocalizationKey;
            return string.IsNullOrEmpty(key) ? string.Empty : SH.ResourceManager.GetString(key, CultureInfo.CurrentCulture) ?? string.Empty;
        }
    }

    extension<TResponse>(TResponse localizableResponse)
        where TResponse : Web.Response.Response, ILocalizableResponse
    {
        public string GetLocalizationMessageOrMessage()
        {
            return localizableResponse.GetLocalizationMessageOrDefault() ?? localizableResponse.Message;
        }
    }
}