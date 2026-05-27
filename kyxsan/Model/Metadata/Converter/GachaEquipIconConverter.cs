//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Xaml.Data.Converter;
using kyxsan.Web.Endpoint.kyxsan;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class GachaEquipIconConverter : ValueConverter<string, Uri>, IIconNameToUriConverter
{
    public static Uri IconNameToUri(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return default!;
        }

        return StaticResourcesEndpoints.StaticRaw("GachaEquipIcon", $"UI_Gacha_{CommonNameExtractor.ExtractUIName(name)}.png").ToUri();
    }

    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}