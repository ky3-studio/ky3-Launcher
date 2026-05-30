//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Intrinsic.Format;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace kyxsan.Model.Metadata.Converter;

internal static class FightPropertyFormat
{
    public static NameValue<string> ToNameValue(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, FormatValue(property, value));
    }

    public static NameValue<string> ToNameValue(ReliquaryProperty baseProperty)
    {
        return new(baseProperty.PropertyType.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, baseProperty.Value);
    }

    public static NameStringValue ToNameStringValue(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, FormatValue(property, value));
    }

    public static NameDescription ToNameDescription(FightProperty property, float value)
    {
        return new(property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty, FormatValue(property, value));
    }

    public static ParameterDescription ToParameterDescription(FightProperty property, float value)
    {
        return new(FormatValue(property, value), property.GetLocalizedDescription(SH.ResourceManager) ?? string.Empty);
    }

    public static string FormatValue(FightProperty property, float value)
    {
        return FormatValue(property.GetFormatMethod(), value);
    }

    public static string FormatValue(FormatMethod method, float value)
    {
        return method switch
        {
            FormatMethod.Integer => $"{MathF.Round(value, MidpointRounding.AwayFromZero)}",
            FormatMethod.Percent => $"{value:P1}",
            _ => $"{value}",
        };
    }
}