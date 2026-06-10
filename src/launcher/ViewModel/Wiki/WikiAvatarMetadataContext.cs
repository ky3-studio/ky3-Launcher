//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Wiki;

internal sealed class WikiAvatarMetadataContext : IMetadataContext,
    IMetadataDictionaryLevelAvatarGrowCurveSource,
    IMetadataDictionaryIdDictionaryLevelAvatarPromoteSource,
    IMetadataDictionaryIdMaterialSource,
    IMetadataDictionaryIdHyperLinkNameSource,
    IMetadataArrayAvatarSource
{
    public ImmutableDictionary<Level, TypeValueCollection<GrowCurveType, float>> LevelDictionaryAvatarGrowCurveMap { get; set; } = default!;

    public ImmutableDictionary<PromoteId, ImmutableDictionary<PromoteLevel, Promote>> IdDictionaryAvatarLevelPromoteMap { get; set; } = default!;

    public ImmutableDictionary<MaterialId, Material> IdMaterialMap { get; set; } = default!;

    public ImmutableDictionary<HyperLinkNameId, HyperLinkName> IdHyperLinkNameMap { get; set; } = default!;

    public ImmutableArray<Avatar> Avatars { get; set; }
}