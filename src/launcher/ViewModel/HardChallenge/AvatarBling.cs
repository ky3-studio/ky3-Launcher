//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Metadata.Avatar;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;

namespace kyxsan.ViewModel.HardChallenge;

internal sealed class AvatarBling : AvatarView
{
    private AvatarBling(bool isPlus, Avatar metaAvatar)
        : base(metaAvatar)
    {
        IsPlus = isPlus;
    }

    public bool IsPlus { get; }

    public static AvatarBling Create(HardChallengeBlingAvatar avatar, HardChallengeMetadataContext context)
    {
        return new(avatar.IsPlus, context.GetAvatar(avatar.AvatarId));
    }

    public static AvatarBling Create(HardChallengeBlingAvatar avatar, Avatar metaAvatar)
    {
        return new(avatar.IsPlus, metaAvatar);
    }
}