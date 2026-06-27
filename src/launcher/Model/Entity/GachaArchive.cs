using Launcher.Core.Database.Abstraction;
using Launcher.UI.Xaml.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Launcher.Model.Entity;

[Table("gacha_archives")]
internal sealed partial class GachaArchive : ISelectable, IPropertyValuesProvider
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public string Uid { get; set; } = default!;

    public bool IsSelected { get; set; }

    public static GachaArchive Create(string uid)
    {
        return new() { Uid = uid };
    }
}
