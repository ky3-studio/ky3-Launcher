// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Database.Abstraction;
using kyxsan.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kyxsan.Model.Entity;

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
