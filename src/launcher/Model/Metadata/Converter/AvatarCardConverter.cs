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

internal sealed partial class AvatarCardConverter : ValueConverter<string, Uri>, IIconNameToUriConverter
{
    private const string CostumeCard = "UI_AvatarIcon_Costume_Card.png";
    private static readonly Uri UIAvatarIconCostumeCard = StaticResourcesEndpoints.StaticRaw("AvatarCard", CostumeCard).ToUri();

    public static Uri IconNameToUri(string name)
    {
        return string.IsNullOrEmpty(name)
            ? UIAvatarIconCostumeCard
            : StaticResourcesEndpoints.StaticRaw("AvatarCard", $"{name}_Card.png").ToUri();
    }

    public override Uri Convert(string from)
    {
        return IconNameToUri(from);
    }
}