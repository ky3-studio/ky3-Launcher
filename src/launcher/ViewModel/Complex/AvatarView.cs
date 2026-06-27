//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Converter;

namespace Launcher.ViewModel.Complex;

internal class AvatarView : CollocationView
{
    public AvatarView(Avatar avatar, double rate, double? lastRate = default)
        : base(rate, lastRate)
    {
        Name = avatar.Name;
        Icon = AvatarIconConverter.IconNameToUri(avatar.Icon);
        SideIcon = AvatarSideIconConverter.IconNameToUri(avatar.SideIcon);
        Quality = avatar.Quality;
    }

    public override string Name { get; }

    public override Uri Icon { get; }

    public override QualityType Quality { get; }

    public Uri SideIcon { get; }
}