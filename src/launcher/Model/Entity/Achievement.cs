//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Entity.Abstraction;
using Launcher.Model.InterChange.Achievement;
using Launcher.Model.Intrinsic;
using Launcher.Model.Primitive;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Launcher.Model.Entity;

[Table("achievements")]
internal sealed class Achievement : IAppDbEntityHasArchive,
    IEquatable<Achievement>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public Guid ArchiveId { get; set; }

    [ForeignKey(nameof(ArchiveId))]
    public AchievementArchive Archive { get; set; } = default!;

    public uint Id { get; set; }

    public uint Current { get; set; }

    public DateTimeOffset Time { get; set; }

    public AchievementStatus Status { get; set; }

    public static Achievement Create(Guid archiveId, AchievementId id)
    {
        return new()
        {
            ArchiveId = archiveId,
            Id = id,
            Current = 0,
            Time = DateTimeOffset.MinValue,
        };
    }

    public static Achievement Create(Guid archiveId, UIAFItem uiaf)
    {
        return new()
        {
            ArchiveId = archiveId,
            Id = uiaf.Id,
            Current = uiaf.Current,
            Status = uiaf.Status,
            Time = DateTimeOffset.FromUnixTimeSeconds(uiaf.Timestamp).ToLocalTime(),
        };
    }

    public bool Equals(Achievement? other)
    {
        if (other is null)
        {
            return false;
        }

        return ArchiveId == other.ArchiveId
            && Id == other.Id
            && Current == other.Current
            && Status == other.Status
            && Time == other.Time;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Achievement);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ArchiveId, Id, Current, Status, Time);
    }
}
