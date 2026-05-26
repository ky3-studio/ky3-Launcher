//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace kyxsan.Web.kyxsan.SpiralAbyss.Post;

internal sealed class SimpleAvatar
{
    public SimpleAvatar(DetailedCharacter character)
    {
        AvatarId = character.Base.Id;
        WeaponId = character.Weapon.Id;
        ReliquarySetIds = character.Relics.Select(r => r.ReliquarySet.Id);
        ActivedConstellationNumber = character.Base.ActivedConstellationNum;
    }

    /// <summary>
    /// 角色 Id
    /// </summary>
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// 武器 Id
    /// </summary>
    public WeaponId WeaponId { get; set; }

    /// <summary>
    /// 圣遗物套装Id
    /// </summary>
    public IEnumerable<ReliquarySetId> ReliquarySetIds { get; set; }

    /// <summary>
    /// 命座
    /// </summary>
    public int ActivedConstellationNumber { get; set; }
}