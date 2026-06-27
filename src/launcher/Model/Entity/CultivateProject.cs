// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.Database.Abstraction;
using Launcher.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Launcher.Model.Entity;

[Table("cultivate_projects")]
internal sealed partial class CultivateProject : ISelectable, IPropertyValuesProvider
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public bool IsSelected { get; set; }

    public string Name { get; set; } = default!;

    public TimeSpan ServerTimeZoneOffset { get; set; }

    public static CultivateProject From(string name, in TimeSpan serverTimeOffset)
    {
        return new()
        {
            Name = name,
            ServerTimeZoneOffset = serverTimeOffset,
        };
    }
}
