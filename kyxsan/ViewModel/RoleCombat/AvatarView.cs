//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.RoleCombat;

namespace kyxsan.ViewModel.RoleCombat;

internal class AvatarView : INameIconSide<Uri>
{
    protected AvatarView(Avatar metaAvatar)
    {
        Name = metaAvatar.Name;
        Icon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.Icon);
        SideIcon = Model.Metadata.Converter.AvatarIconConverter.IconNameToUri(metaAvatar.SideIcon);
        Quality = metaAvatar.Quality;
    }

    private AvatarView(RoleCombatAvatar roleCombatAvatar, Avatar metaAvatar)
        : this(metaAvatar)
    {
        Type = roleCombatAvatar.AvatarType;
    }

    public string Name { get; }

    public Uri Icon { get; }

    public Uri SideIcon { get; }

    public QualityType Quality { get; }

    public RoleCombatAvatarType Type { get; }

    public static AvatarView Create(Avatar metaAvatar)
    {
        return new(metaAvatar);
    }

    public static AvatarView Create(RoleCombatAvatar roleCombatAvatar, RoleCombatMetadataContext context)
    {
        return new(roleCombatAvatar, context.GetAvatar(roleCombatAvatar.AvatarId));
    }

    public static AvatarView Create(RoleCombatAvatar roleCombatAvatar, Avatar metaAvatar)
    {
        return new(roleCombatAvatar, metaAvatar);
    }
}