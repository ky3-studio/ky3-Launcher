//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Globalization;

namespace kyxsan.Web.Endpoint.kyxsan;

internal interface IInfrastructureMetadataEndpoints : IInfrastructureRootAccess
{
    string Metadata(string locale, string fileName)
    {
        return $"{Root}/metadata/Genshin/{locale}/{fileName}";
    }

    string Metadata(string template, string locale, string fileName)
    {
        return string.Format(CultureInfo.CurrentCulture, template, $"Genshin/{locale}/{fileName}");
    }

    string MetadataTemplate()
    {
        return $"{Root}/metadata/template";
    }
}