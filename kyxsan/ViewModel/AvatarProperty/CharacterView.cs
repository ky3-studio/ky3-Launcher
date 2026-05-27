//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Primitive;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.Avatar;
using MetaAvatar = kyxsan.Model.Metadata.Avatar.Avatar;

namespace kyxsan.ViewModel.AvatarProperty;

internal sealed class CharacterView
{
    public CharacterView(Character apiCharacter, MetaAvatar metaAvatar)
    {
        Id = apiCharacter.Id;
        Name = apiCharacter.Name;
        Level = apiCharacter.Level;
        LevelFormatted = LevelFormat.Format(apiCharacter.Level);
        Element = apiCharacter.Element;
        Rarity = apiCharacter.Rarity;
        ActivedConstellationNum = apiCharacter.ActivedConstellationNum;
        Icon = AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = AvatarSideIconConverter.IconNameToUri(metaAvatar.SideIcon);
    }

    public AvatarId Id { get; }

    public string Name { get; }

    public Level Level { get; }

    public string LevelFormatted { get; }

    public ElementName Element { get; }

    public QualityType Rarity { get; }

    public int ActivedConstellationNum { get; }

    public Uri Icon { get; }

    public Uri SideIcon { get; }
}
