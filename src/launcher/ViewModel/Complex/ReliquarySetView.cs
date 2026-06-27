//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Metadata.Converter;
using Launcher.Model.Primitive;
using Launcher.Web.Launcher.SpiralAbyss;
using System.Collections.Immutable;
using System.Text;

namespace Launcher.ViewModel.Complex;

/// <summary>
/// Ê¥ÒÅÎïÌ××°
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
                nameBuilder.Append(set.Count).Append('¡Á').Append(metaSet.Name).Append('+');
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