//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI;
using kyxsan.Core;
using kyxsan.Model.Intrinsic;
using kyxsan.UI.Xaml.Control.Theme;
using kyxsan.UI.Xaml.Data.Converter;
using System.Collections.Frozen;
using Windows.UI;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class QualityColorConverter : ValueConverter<QualityType, Color>
{
    private static readonly FrozenDictionary<string, QualityType> LocalizedNameToQualityType = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityWhite, QualityType.QUALITY_WHITE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityGreen, QualityType.QUALITY_GREEN),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityBlue, QualityType.QUALITY_BLUE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityPurple, QualityType.QUALITY_PURPLE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityOrange, QualityType.QUALITY_ORANGE),
        KeyValuePair.Create(SH.ModelIntrinsicItemQualityRed, QualityType.QUALITY_ORANGE_SP),
    ]);

    private static readonly FrozenDictionary<QualityType, Color> QualityTypeToColor = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(QualityType.QUALITY_WHITE, KnownColors.White),
        KeyValuePair.Create(QualityType.QUALITY_GREEN, KnownColors.Green),
        KeyValuePair.Create(QualityType.QUALITY_BLUE, KnownColors.Blue),
        KeyValuePair.Create(QualityType.QUALITY_PURPLE, KnownColors.Purple),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE, KnownColors.Orange),
        KeyValuePair.Create(QualityType.QUALITY_ORANGE_SP, KnownColors.Orange),
    ]);

    public static Color QualityNameToColor(string qualityName)
    {
        return QualityToColor(LocalizedNameToQualityType.GetValueOrDefault(qualityName));
    }

    public static Color QualityToColor(QualityType quality)
    {
        return QualityTypeToColor.GetValueOrDefault(quality, Colors.Transparent);
    }

    public override Color Convert(QualityType from)
    {
        return QualityToColor(from);
    }
}