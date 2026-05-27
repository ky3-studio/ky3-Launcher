//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Primitive;
using kyxsan.Web.kyxsan.SpiralAbyss;
using System.Collections.Immutable;
using System.Text;

namespace kyxsan.ViewModel.Complex;

/// <summary>
/// 圣遗物套装
/// </summary>
internal sealed class ReliquarySetView : RateAndDelta
{
    public ReliquarySetView(ImmutableDictionary<ExtendedEquipAffixId, Model.Metadata.Reliquary.ReliquarySet> idReliquarySetMap, ItemRate<ReliquarySets, double> setRate, ItemRate<ReliquarySets, double>? lastSetRate)
        : base(setRate.Rate, lastSetRate?.Rate)
    {
        ReliquarySets sets = setRate.Item;

        if (sets is [_, ..])
        {
            StringBuilder nameBuilder = new();
            List<Uri> icons = new(2);
            foreach (ReliquarySet set in sets)
            {
                Model.Metadata.Reliquary.ReliquarySet metaSet = idReliquarySetMap[set.EquipAffixId];
                nameBuilder.Append(set.Count).Append('×').Append(metaSet.Name).Append('+');
                icons.Add(RelicIconConverter.IconNameToUri(metaSet.Icon));
            }

            Name = nameBuilder.ToString(0, nameBuilder.Length - 1);
            Icons = icons;
        }
        else
        {
            Name = SH.ViewModelComplexReliquarySetViewEmptyName;
        }
    }

    public string Name { get; }

    public List<Uri>? Icons { get; }
}