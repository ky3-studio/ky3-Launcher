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

internal sealed class SpiralAbyssEntryConfiguration : IEntityTypeConfiguration<SpiralAbyssEntry>
{
    public void Configure(EntityTypeBuilder<SpiralAbyssEntry> builder)
    {
        builder.Property(e => e.SpiralAbyss)
            .HasColumnType(SqliteTypeNames.Text)
            .HasConversion<JsonTextValueConverter<Web.Hoyolab.Takumi.GameRecord.SpiralAbyss.SpiralAbyss>>();
    }
}