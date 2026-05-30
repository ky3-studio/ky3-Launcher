//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace kyxsan.Web.kyxsan.RoleCombat.Post;

internal sealed class SimpleRoleCombatRecord
{
    public SimpleRoleCombatRecord(string uid, ImmutableArray<uint> backupAvatars, uint scheduleId)
    {
        Version = 1;
        Uid = uid;
        Identity = "ky3 Launcher"; // hardcoded Identity name
        BackupAvatars = backupAvatars;
        ScheduleId = scheduleId;
    }

    public uint Version { get; set; }

    public string Uid { get; set; }

    public string Identity { get; set; }

    public ImmutableArray<uint> BackupAvatars { get; set; }

    public uint ScheduleId { get; set; }
}