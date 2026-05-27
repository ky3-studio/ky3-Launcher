//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Primitive;
using kyxsan.Web.kyxsan.SpiralAbyss;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Complex;

internal sealed class TeamAppearanceView
{
    public TeamAppearanceView(TeamAppearance teamRank, ImmutableDictionary<AvatarId, Avatar> idAvatarMap)
    {
        Floor = SH.FormatModelBindingkyxsanComplexRankFloor(teamRank.Floor);
        Up = teamRank.Up.SelectAsArray(static (teamRate, index, idAvatarMap) => new Team(teamRate, idAvatarMap, index + 1), idAvatarMap);
        Down = teamRank.Down.SelectAsArray(static (teamRate, index, idAvatarMap) => new Team(teamRate, idAvatarMap, index + 1), idAvatarMap);
    }

    public string Floor { get; }

    public ImmutableArray<Team> Up { get; }

    public ImmutableArray<Team> Down { get; }
}