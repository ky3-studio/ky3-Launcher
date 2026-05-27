//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Primitive;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.HardChallenge;

internal sealed class MonsterView
{
    private MonsterView(HardChallengeMonster monster, HardChallengeMetadataContext context)
    {
        Name = monster.Name;
        Level = LevelFormat.Format(monster.Level);
        Icon = monster.Icon;
        Descriptions = [.. monster.Descriptions.Where(static d => !string.IsNullOrEmpty(d))];
        Tags = monster.Tags;
    }

    public string Name { get; }

    public string Level { get; }

    public Uri Icon { get; }

    public ImmutableArray<string> Descriptions { get; }

    public ImmutableArray<HardChallengeMonsterTag> Tags { get; }

    public static MonsterView Create(HardChallengeMonster monster, HardChallengeMetadataContext context)
    {
        return new(monster, context);
    }
}