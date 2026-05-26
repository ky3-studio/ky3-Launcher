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
using System.Globalization;

namespace kyxsan.ViewModel.HardChallenge;

internal sealed class AvatarDamage : AvatarView
{
    private AvatarDamage(HardChallengeBestAvatar avatar, Avatar metaAvatar)
        : base(metaAvatar)
    {
        Value = avatar.Dps;
        Type = avatar.Type.GetLocalizedDescription(SH.ResourceManager, CultureInfo.CurrentCulture);
    }

    public int Value { get; }

    public string? Type { get; }

    public static AvatarDamage Create(HardChallengeBestAvatar avatar, HardChallengeMetadataContext context)
    {
        return new(avatar, context.GetAvatar(avatar.AvatarId));
    }

    public static AvatarDamage Create(HardChallengeBestAvatar avatar, Avatar metaAvatar)
    {
        return new(avatar, metaAvatar);
    }
}