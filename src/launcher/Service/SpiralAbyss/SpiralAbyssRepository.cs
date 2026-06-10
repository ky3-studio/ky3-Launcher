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

namespace kyxsan.Service.SpiralAbyss;

[Service(ServiceLifetime.Singleton, typeof(ISpiralAbyssRepository))]
internal sealed partial class SpiralAbyssRepository : ISpiralAbyssRepository
{
    [GeneratedConstructor]
    public partial SpiralAbyssRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public FrozenDictionary<uint, SpiralAbyssEntry> GetSpiralAbyssEntryMapByUid(string uid)
    {
        return this.Query(query => query
            .Where(s => s.Uid == uid)
            .OrderByDescending(s => s.ScheduleId)
            .ToFrozenDictionary(e => e.ScheduleId));
    }

    public void UpdateSpiralAbyssEntry(SpiralAbyssEntry entry)
    {
        this.Update(entry);
    }

    public void AddSpiralAbyssEntry(SpiralAbyssEntry entry)
    {
        this.Add(entry);
    }
}