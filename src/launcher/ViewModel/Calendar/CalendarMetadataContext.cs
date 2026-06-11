using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Calendar;

internal sealed class CalendarMetadataContext : IMetadataContext,
    IMetadataArrayAvatarSource,
    IMetadataArrayWeaponSource,
    IMetadataDictionaryIdMaterialSource
{
    public ImmutableArray<Avatar> Avatars { get; set; }

    public ImmutableArray<Weapon> Weapons { get; set; }

    public ImmutableDictionary<Model.Primitive.MaterialId, Model.Metadata.Item.Material> IdMaterialMap { get; set; } = default!;
}
