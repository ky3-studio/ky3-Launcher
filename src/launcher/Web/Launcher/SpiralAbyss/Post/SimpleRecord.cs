//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Web.Hoyolab.Takumi.GameRecord.Avatar;
using System.Collections.Immutable;

namespace Launcher.Web.Launcher.SpiralAbyss.Post;

internal sealed class SimpleRecord
{
    public SimpleRecord(string uid, ImmutableArray<DetailedCharacter> characters, Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss spiralAbyss, string? reservedUserName)
    {
        Uid = uid;
        Identity = "ky3 Launcher"; // hardcoded Identity name
        SpiralAbyss = new(spiralAbyss);
        Avatars = characters.SelectAsArray(static a => new SimpleAvatar(a));
        ReservedUserName = reservedUserName;
    }

    public string Uid { get; set; }

    public string Identity { get; set; }

    public string? ReservedUserName { get; set; }

    public SimpleSpiralAbyss SpiralAbyss { get; set; }

    public ImmutableArray<SimpleAvatar> Avatars { get; set; }
}