//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;

namespace kyxsan.Service.GachaLog.Factory;

internal readonly struct TypedWishSummaryBuilderContext
{
    public readonly IServiceProvider ServiceProvider;
    public readonly ITaskContext TaskContext;
    public readonly string Name;
    public readonly int GuaranteeOrangeThreshold;
    public readonly int GuaranteePurpleThreshold;
    public readonly Func<GachaType, bool> TypeEvaluator;
    public readonly GachaDistributionType DistributionType;

    private static readonly Func<GachaType, bool> IsStandardWish = type => type is GachaType.Standard;
    private static readonly Func<GachaType, bool> IsAvatarEventWish = type => type is GachaType.AvatarEvent or GachaType.AvatarEvent2;
    private static readonly Func<GachaType, bool> IsWeaponEventWish = type => type is GachaType.WeaponEvent;
    private static readonly Func<GachaType, bool> IsChronicledWish = type => type is GachaType.Chronicled;

    public TypedWishSummaryBuilderContext(
        IServiceProvider serviceProvider,
        string name,
        int guaranteeOrangeThreshold,
        int guaranteePurpleThreshold,
        Func<GachaType, bool> typeEvaluator,
        GachaDistributionType distributionType)
    {
        ServiceProvider = serviceProvider;
        TaskContext = serviceProvider.GetRequiredService<ITaskContext>();
        Name = name;
        GuaranteeOrangeThreshold = guaranteeOrangeThreshold;
        GuaranteePurpleThreshold = guaranteePurpleThreshold;
        TypeEvaluator = typeEvaluator;
        DistributionType = distributionType;
    }

    public static TypedWishSummaryBuilderContext StandardWish(GachaStatisticsFactoryContext context)
    {
        return new(context.ServiceProvider, SH.ServiceGachaLogFactoryPermanentWishName, 90, 10, IsStandardWish, GachaDistributionType.Standard);
    }

    public static TypedWishSummaryBuilderContext AvatarEventWish(GachaStatisticsFactoryContext context)
    {
        return new(context.ServiceProvider, SH.ServiceGachaLogFactoryAvatarWishName, 90, 10, IsAvatarEventWish, GachaDistributionType.AvatarEvent);
    }

    public static TypedWishSummaryBuilderContext WeaponEventWish(GachaStatisticsFactoryContext context)
    {
        return new(context.ServiceProvider, SH.ServiceGachaLogFactoryWeaponWishName, 80, 10, IsWeaponEventWish, GachaDistributionType.WeaponEvent);
    }

    public static TypedWishSummaryBuilderContext ChronicledWish(GachaStatisticsFactoryContext context)
    {
        return new(context.ServiceProvider, SH.ServiceGachaLogFactoryChronicledWishName, 90, 10, IsChronicledWish, GachaDistributionType.Chronicled);
    }

    public TypedWishSummaryBuilder CreateBuilder()
    {
        return new(this);
    }
}
