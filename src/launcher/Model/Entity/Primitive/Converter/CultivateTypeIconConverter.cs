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
using kyxsan.UI.Xaml.Data.Converter;
using System.Collections.Frozen;

namespace kyxsan.Model.Entity.Primitive.Converter;

internal sealed partial class CultivateTypeIconConverter : ValueConverter<CultivateType, Uri>
{
    private static readonly FrozenDictionary<string, CultivateType> LocalizedNameToCultivateType = WinRTAdaptive.ToFrozenDictionary(
    [
        KeyValuePair.Create(SH.ModelEntityPrimitiveCultivateTypeAvatarAndSkill, CultivateType.AvatarAndSkill),
        KeyValuePair.Create(SH.ModelEntityPrimitiveCultivateTypeWeapon, CultivateType.Weapon),
        KeyValuePair.Create(SH.ModelEntityPrimitiveCultivateTypeFurniture, CultivateType.Furniture),
    ]);

    public static Uri CultivateTypeNameToIconUri(string cultivateTypeName)
    {
        return CultivateTypeToIconUri(LocalizedNameToCultivateType.GetValueOrDefault(cultivateTypeName));
    }

    public static Uri CultivateTypeToIconUri(CultivateType type)
    {
        string filename = type switch
        {
            CultivateType.AvatarAndSkill => "UI_BtnIcon_PlayerGirl.png",
            CultivateType.Weapon => "UI_BagTabIcon_Weapon.png",
            CultivateType.Furniture => "UI_BtnIcon_Homeworld.png",
            _ => throw kyxsanException.NotSupported(),
        };

        return $"ms-appx:///Resource/Icon/{filename}".ToUri();
    }

    public override Uri Convert(CultivateType from)
    {
        return CultivateTypeToIconUri(from);
    }
}