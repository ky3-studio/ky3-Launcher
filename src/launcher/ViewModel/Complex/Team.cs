//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Primitive;
using Launcher.Web.Launcher.SpiralAbyss;
using System.Collections.Immutable;
using System.Globalization;

namespace Launcher.ViewModel.Complex;

internal sealed partial class Team : List<AvatarView>
{
    public Team(ItemRate<string, int> team, ImmutableDictionary<AvatarId, Avatar> idAvatarMap, int rank)
        : base(4)
    {
        ReadOnlySpan<char> itemSpan = team.Item.AsSpan();
        foreach (Range range in itemSpan.Split(','))
        {
            uint id = uint.Parse(itemSpan[range], CultureInfo.InvariantCulture);
            Add(new(idAvatarMap[id], 0));
        }

        AddRange(new AvatarView[4 - Count]);

        UpCount = SH.FormatModelBindingLauncherTeamUpCount(team.Rate);
        Rank = rank;
    }

    public string UpCount { get; }

    public int Rank { get; set; }
}