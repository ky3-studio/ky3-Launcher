//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Core.ExceptionService;
using kyxsan.Model.Intrinsic;
using kyxsan.UI.Xaml.Data.Converter;
using kyxsan.Web.Endpoint.kyxsan;
using System.Collections.Frozen;

namespace kyxsan.Model.Metadata.Converter;

internal sealed partial class WeaponTypeIconConverter : ValueConverter<WeaponType, Uri>
{
    private static readonly FrozenDictionary<string, WeaponType> LocalizedNameToWeaponType = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelIntrinsicWeaponTypeSwordOneHand, WeaponType.WEAPON_SWORD_ONE_HAND),
        KeyValuePair.Create(SH.ModelIntrinsicWeaponTypeBow, WeaponType.WEAPON_BOW),
        KeyValuePair.Create(SH.ModelIntrinsicWeaponTypePole, WeaponType.WEAPON_POLE),
        KeyValuePair.Create(SH.ModelIntrinsicWeaponTypeClaymore, WeaponType.WEAPON_CLAYMORE),
        KeyValuePair.Create(SH.ModelIntrinsicWeaponTypeCatalyst, WeaponType.WEAPON_CATALYST),
    ]);

    public static Uri WeaponTypeNameToIconUri(string weaponTypeName)
    {
        return WeaponTypeToIconUri(LocalizedNameToWeaponType.GetValueOrDefault(weaponTypeName));
    }

    public static Uri WeaponTypeToIconUri(WeaponType type)
    {
        string weapon = type switch
        {
            WeaponType.WEAPON_SWORD_ONE_HAND => "01",
            WeaponType.WEAPON_BOW => "02",
            WeaponType.WEAPON_POLE => "03",
            WeaponType.WEAPON_CLAYMORE => "04",
            WeaponType.WEAPON_CATALYST => "Catalyst_MD",
            _ => throw kyxsanException.NotSupported(),
        };

        return StaticResourcesEndpoints.StaticRaw("Skill", $"Skill_A_{weapon}.png").ToUri();
    }

    public override Uri Convert(WeaponType from)
    {
        return WeaponTypeToIconUri(from);
    }
}