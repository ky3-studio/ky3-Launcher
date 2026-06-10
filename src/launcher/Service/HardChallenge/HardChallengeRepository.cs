//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Entity;
using kyxsan.Service.Abstraction;
using System.Collections.Frozen;

namespace kyxsan.Service.HardChallenge;

[Service(ServiceLifetime.Singleton, typeof(IHardChallengeRepository))]
internal sealed partial class HardChallengeRepository : IHardChallengeRepository
{
    [GeneratedConstructor]
    public partial HardChallengeRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public FrozenDictionary<uint, HardChallengeEntry> GetHardChallengeMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToFrozenDictionary(e => e.ScheduleId));
    }

    public void UpdateHardChallengeEntry(HardChallengeEntry entry)
    {
        this.Update(entry);
    }

    public void AddHardChallengeEntry(HardChallengeEntry entry)
    {
        this.Add(entry);
    }
}