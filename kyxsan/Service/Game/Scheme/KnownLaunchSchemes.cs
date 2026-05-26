//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;
using System.Collections.Immutable;

namespace kyxsan.Service.Game.Scheme;

internal static class KnownLaunchSchemes
{
    // 国服官服
    private static readonly LaunchScheme ServerChineseChannel00SubChannel00 = new LaunchSchemeChinese(ChannelType.Default, SubChannelType.Default);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel00 = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel01 = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerChineseChannel01SubChannel02 = new LaunchSchemeChinese(ChannelType.Official, SubChannelType.NoTapTap);

    // 国服B服
    private static readonly LaunchScheme ServerChineseChannel14SubChannel00 = new LaunchSchemeBilibili(SubChannelType.Default);

    // 国际服
    private static readonly LaunchScheme ServerOverseaChannel00SubChannel00 = new LaunchSchemeOversea(ChannelType.Default, SubChannelType.Default);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel00 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Default);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel01 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Official);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel03 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Epic);
    private static readonly LaunchScheme ServerOverseaChannel01SubChannel06 = new LaunchSchemeOversea(ChannelType.Official, SubChannelType.Google);

    public static ImmutableArray<LaunchScheme> Values { get; } =
    [
        ServerChineseChannel00SubChannel00,
        ServerChineseChannel01SubChannel00,
        ServerChineseChannel01SubChannel01,
        ServerChineseChannel01SubChannel02,
        ServerChineseChannel14SubChannel00,
        ServerOverseaChannel00SubChannel00,
        ServerOverseaChannel01SubChannel00,
        ServerOverseaChannel01SubChannel01,
        ServerOverseaChannel01SubChannel03,
        ServerOverseaChannel01SubChannel06,
    ];

    public static ImmutableArray<LaunchScheme> BetaValues { get; } =
    [
        ServerChineseChannel01SubChannel01,
        ServerOverseaChannel01SubChannel00,
    ];

    public static IEnumerable<LaunchScheme> EnumerateNotCompatOnly(bool isOversea)
    {
        return Values.Where(scheme => scheme.IsNotCompatOnly && scheme.IsOversea == isOversea);
    }
}