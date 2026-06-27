//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Primitive;
using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;

namespace Launcher.Web.Launcher.SpiralAbyss.Post;

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
    /// ½ÇÉ« Id
    /// </summary>
    public AvatarId AvatarId { get; set; }

    /// <summary>
    /// ÎäÆ÷ Id
    /// </summary>
    public WeaponId WeaponId { get; set; }

    /// <summary>
    /// Ê¥ÒÅÎïÌ××°Id
    /// </summary>
    public IEnumerable<ReliquarySetId> ReliquarySetIds { get; set; }

    /// <summary>
    /// Ãü×ù
    /// </summary>
    public int ActivedConstellationNumber { get; set; }
}