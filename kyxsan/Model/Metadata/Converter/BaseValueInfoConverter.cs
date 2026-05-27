//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using kyxsan.ViewModel.Wiki;
using System.Collections.Immutable;

namespace kyxsan.Model.Metadata.Converter;

internal static class BaseValueInfoConverter
{
    public static bool GetPromoted(Level level, PromoteLevel promoteLevel)
    {
        return (level.Value, promoteLevel.Value) switch
        {
            (20U, 0U) or (40U, 1U) or (50U, 2U) or (60U, 3U) or (70U, 4U) or (80U, 5U) => false,
            _ => true,
        };
    }

    public static PromoteLevel GetPromoteLevel(Level level, Level maxLevel, bool promoted)
    {
        if (maxLevel <= 70U && level == 70U)
        {
            return 4U;
        }

        if (promoted)
        {
            return level.Value switch
            {
                >= 80U => 6U,
                >= 70U => 5U,
                >= 60U => 4U,
                >= 50U => 3U,
                >= 40U => 2U,
                >= 20U => 1U,
                _ => 0U,
            };
        }

        return level.Value switch
        {
            > 80U => 6U,
            > 70U => 5U,
            > 60U => 4U,
            > 50U => 3U,
            > 40U => 2U,
            > 20U => 1U,
            _ => 0U,
        };
    }

    public static NameValue<string> ToNameValue(PropertyCurveValue propValue, Level level, PromoteLevel promoteLevel, BaseValueInfoMetadataContext metadataContext)
    {
        float value = propValue.Value * metadataContext.GrowCurveMap[level].GetValueOrDefault(propValue.Type);
        if (metadataContext.PromoteMap is not null)
        {
            value += metadataContext.PromoteMap[promoteLevel].AddProperties.GetValueOrDefault(propValue.Property);
        }

        return FightPropertyFormat.ToNameValue(propValue.Property, value);
    }

    public static ImmutableArray<NameValue<string>> ToNameValues(ImmutableArray<PropertyCurveValue> propValues, Level level, PromoteLevel promoteLevel, BaseValueInfoMetadataContext metadataContext)
    {
        return propValues.SelectAsArray(static (propValue, state) => ToNameValue(propValue, state.level, state.promoteLevel, state.metadataContext), (level, promoteLevel, metadataContext));
    }

    public static ImmutableArray<NameValue<string>> ToNameValues(ImmutableArray<PropertyCurveValue> propValues, Level level, Level maxLevel, bool promoted, BaseValueInfoMetadataContext metadataContext)
    {
        return ToNameValues(propValues, level, GetPromoteLevel(level, maxLevel, promoted), metadataContext);
    }
}