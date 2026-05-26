//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Model.Intrinsic;
using kyxsan.UI.Xaml.Data.Converter;
using kyxsan.Web.Endpoint.kyxsan;
using System.Collections.Frozen;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class ElementNameIconConverter : ValueConverter<string, Uri>
{
    private static readonly FrozenDictionary<string, string> LocalizedNameToElementIconName = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicElementNameElec, "Electric"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameFire, "Fire"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameGrass, "Grass"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameIce, "Ice"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameRock, "Rock"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWater, "Water"),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWind, "Wind"),
    ]);

    private static readonly FrozenDictionary<string, ElementType> LocalizedNameToElementType = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicElementNameElec, ElementType.Electric),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameFire, ElementType.Fire),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameGrass, ElementType.Grass),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameIce, ElementType.Ice),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameRock, ElementType.Rock),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWater, ElementType.Water),
        KeyValuePair.Create(SH.ModelIntrinsicElementNameWind, ElementType.Wind),
    ]);

    public static Uri ElementNameToUri(string elementName)
    {
        string? element = LocalizedNameToElementIconName.GetValueOrDefault(elementName);

        return string.IsNullOrEmpty(element)
            ? StaticResourcesEndpoints.UIIconNone
            : StaticResourcesEndpoints.StaticRaw("IconElement", $"UI_Icon_Element_{element}.png").ToUri();
    }

    public static ElementType ElementNameToElementType(string elementName)
    {
        return LocalizedNameToElementType.GetValueOrDefault(elementName);
    }

    public override Uri Convert(string from)
    {
        return ElementNameToUri(from);
    }
}