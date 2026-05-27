//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using kyxsan.Model.Entity;
using kyxsan.Service.Abstraction;
using System.Collections.Immutable;

namespace kyxsan.Service.DailyNote;

[Service(ServiceLifetime.Singleton, typeof(IDailyNoteRepository))]
internal sealed partial class DailyNoteRepository : IDailyNoteRepository
{
    [GeneratedConstructor]
    public partial DailyNoteRepository(IServiceProvider serviceProvider);

    public partial IServiceProvider ServiceProvider { get; }

    public bool ContainsUid(string uid)
    {
        return this.Query(query => query.Any(n => n.Uid == uid));
    }

    public void AddDailyNoteEntry(DailyNoteEntry entry)
    {
        this.Add(entry);
    }

    public void DeleteDailyNoteEntryById(Guid entryId)
    {
        this.DeleteByInnerId(entryId);
    }

    public void UpdateDailyNoteEntry(DailyNoteEntry entry)
    {
        this.Update(entry);
    }

    public ImmutableArray<DailyNoteEntry> GetDailyNoteEntryImmutableArrayIncludingUser()
    {
        return this.ImmutableArray(query => query.Include(n => n.User));
    }
}