//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using kyxsan.Service.Game.Configuration;

namespace kyxsan.Service.Game.Scheme;

internal class LaunchScheme : IEquatable<ChannelOptions>
{
    public string DisplayName
    {
        get
        {
            string name = (Channel, IsOversea) switch
            {
                (ChannelType.Bili, false) => SH.ModelBindingLaunchGameLaunchSchemeBilibili,
                (_, false) => SH.ModelBindingLaunchGameLaunchSchemeChinese,
                (_, true) => SH.ModelBindingLaunchGameLaunchSchemeOversea,
            };

            return $"{name} | {Channel} | {SubChannel}";
        }
    }

    public string Description => GetDescription();

    private string GetDescription()
    {
        return (Channel, SubChannel, IsOversea) switch
        {
            (ChannelType.Default, SubChannelType.Default, false) => SH.ServiceLaunchSchemeDescCnDefault,
            (ChannelType.Official, SubChannelType.Default, false) => SH.ServiceLaunchSchemeDescCnOfficialDefault,
            (ChannelType.Official, SubChannelType.Official, false) => SH.ServiceLaunchSchemeDescCnOfficialOfficial,
            (ChannelType.Official, SubChannelType.NoTapTap, false) => SH.ServiceLaunchSchemeDescCnOfficialNoTapTap,
            (ChannelType.Bili, SubChannelType.Default, false) => SH.ServiceLaunchSchemeDescCnBili,
            (ChannelType.Default, SubChannelType.Default, true) => SH.ServiceLaunchSchemeDescOsDefault,
            (ChannelType.Official, SubChannelType.Default, true) => SH.ServiceLaunchSchemeDescOsOfficialDefault,
            (ChannelType.Official, SubChannelType.Official, true) => SH.ServiceLaunchSchemeDescOsOfficialOfficial,
            (ChannelType.Official, SubChannelType.Epic, true) => SH.ServiceLaunchSchemeDescOsEpic,
            (ChannelType.Official, SubChannelType.Google, true) => SH.ServiceLaunchSchemeDescOsGoogle,
            _ => ""
        };
    }

    public ChannelType Channel { get; private protected set; }

    public SubChannelType SubChannel { get; private protected set; }

    public string LauncherId { get; private protected set; } = default!;

    public string GameId { get; private protected set; } = default!;

    public bool IsOversea { get; private protected set; }

    public bool IsNotCompatOnly { get; private protected set; } = true;

    public bool Equals(ChannelOptions other)
    {
        return Channel == other.Channel && SubChannel == other.SubChannel && IsOversea == other.IsOversea;
    }
}