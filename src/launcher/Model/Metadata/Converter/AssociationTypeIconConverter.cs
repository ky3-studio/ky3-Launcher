//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Model.Intrinsic;
using kyxsan.UI.Xaml.Data.Converter;
using kyxsan.Web.Endpoint.kyxsan;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class AssociationTypeIconConverter : ValueConverter<AssociationType, Uri?>
{
    public static Uri? AssociationTypeToIconUri(AssociationType type)
    {
        string? association = type switch
        {
            AssociationType.ASSOC_TYPE_MONDSTADT => "Mengde",
            AssociationType.ASSOC_TYPE_LIYUE => "Liyue",
            AssociationType.ASSOC_TYPE_FATUI => default,
            AssociationType.ASSOC_TYPE_INAZUMA => "Inazuma",
            AssociationType.ASSOC_TYPE_RANGER => default,
            AssociationType.ASSOC_TYPE_SUMERU => "Sumeru",
            AssociationType.ASSOC_TYPE_FONTAINE => "Fontaine",
            AssociationType.ASSOC_TYPE_NATLAN => "Natlan",
            AssociationType.ASSOC_TYPE_SNEZHNAYA => default,
            AssociationType.ASSOC_TYPE_OMNI_SCOURGE => default,
            AssociationType.ASSOC_TYPE_NODKRAI => "Nodkrai",
            _ => throw kyxsanException.NotSupported(),
        };

        return association is null
            ? default
            : StaticResourcesEndpoints.StaticRaw("ChapterIcon", $"UI_ChapterIcon_{association}.png").ToUri();
    }

    public override Uri? Convert(AssociationType from)
    {
        return AssociationTypeToIconUri(from);
    }
}