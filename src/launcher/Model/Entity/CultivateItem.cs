// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Launcher.Model.Entity;

[Table("cultivate_items")]
internal sealed class CultivateItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid EntryId { get; set; }

    [ForeignKey(nameof(EntryId))]
    public CultivateEntry Entry { get; set; } = default!;

    public uint ItemId { get; set; }

    public uint Count { get; set; }

    public bool IsFinished { get; set; }

    public static CultivateItem From(Guid entryId, uint itemId, uint count)
    {
        return new()
        {
            EntryId = entryId,
            ItemId = itemId,
            Count = count,
        };
    }
}
