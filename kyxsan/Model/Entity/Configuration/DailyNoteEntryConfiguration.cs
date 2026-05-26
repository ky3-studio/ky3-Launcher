//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace kyxsan.Model.Entity.Configuration;

internal sealed class DailyNoteEntryConfiguration : IEntityTypeConfiguration<DailyNoteEntry>
{
    public void Configure(EntityTypeBuilder<DailyNoteEntry> builder)
    {
        builder.Property(e => e.DailyNote)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion<JsonTextValueConverter<Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote>>();
    }
}
