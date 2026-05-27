//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.ViewModel.GachaLog;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace kyxsan.Service.GachaLog;

[Service(ServiceLifetime.Transient, typeof(IGachaLogWishCountdownService))]
internal sealed partial class GachaLogWishCountdownService : IGachaLogWishCountdownService
{
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial GachaLogWishCountdownService(IServiceProvider serviceProvider);

    public async ValueTask<WishCountdownBundle> GetWishCountdownBundleAsync(GachaLogWishCountdownServiceMetadataContext context)
    {
        await taskContext.SwitchToBackgroundAsync();
        return SynchronizedGetWishCountdownBundle(context);
    }

    private static WishCountdownBundle SynchronizedGetWishCountdownBundle(GachaLogWishCountdownServiceMetadataContext context)
    {
        Dictionary<uint, Countdown> idToCountdown = [];

        ImmutableArray<Countdown>.Builder orangeAvatarCountdowns = ImmutableArray.CreateBuilder<Countdown>();
        ImmutableArray<Countdown>.Builder purpleAvatarCountdowns = ImmutableArray.CreateBuilder<Countdown>();
        ImmutableArray<Countdown>.Builder orangeWeaponCountdowns = ImmutableArray.CreateBuilder<Countdown>();
        ImmutableArray<Countdown>.Builder purpleWeaponCountdowns = ImmutableArray.CreateBuilder<Countdown>();

        ImmutableArray<GachaEvent> events = [.. context.GachaEvents.OrderByDescending(b => b.From)];
        foreach (ref readonly GachaEvent gachaEvent in events.AsSpan())
        {
            if (gachaEvent.From > DateTimeOffset.Now)
            {
                continue;
            }

            foreach (uint itemId in gachaEvent.UpOrangeList)
            {
                switch (itemId.StringLength)
                {
                    case 8U:
                        if (!AvatarIds.IsStandardWish(itemId))
                        {
                            TrackAvatarItemId(context, idToCountdown, orangeAvatarCountdowns, gachaEvent, itemId);
                        }

                        break;

                    case 5U:
                        if (!WeaponIds.IsOrangeStandardWish(itemId))
                        {
                            TrackWeaponItemId(context, idToCountdown, orangeWeaponCountdowns, gachaEvent, itemId);
                        }

                        break;

                    default:
                        throw kyxsanException.NotSupported();
                }
            }

            foreach (uint itemId in gachaEvent.UpPurpleList)
            {
                switch (itemId.StringLength)
                {
                    case 8U:
                        if (!AvatarIds.IsStandardWish(itemId))
                        {
                            TrackAvatarItemId(context, idToCountdown, purpleAvatarCountdowns, gachaEvent, itemId);
                        }

                        break;
                    case 5U:
                        TrackWeaponItemId(context, idToCountdown, purpleWeaponCountdowns, gachaEvent, itemId);
                        break;
                    default:
                        throw kyxsanException.NotSupported();
                }
            }
        }

        return new()
        {
            OrangeAvatars = orangeAvatarCountdowns.ToImmutable(),
            PurpleAvatars = purpleAvatarCountdowns.ToImmutable(),
            OrangeWeapons = orangeWeaponCountdowns.ToImmutable(),
            PurpleWeapons = purpleWeaponCountdowns.ToImmutable(),
        };
    }

    private static void TrackAvatarItemId(GachaLogWishCountdownServiceMetadataContext context, Dictionary<uint, Countdown> idToCountdown, ImmutableArray<Countdown>.Builder builder, GachaEvent gachaEvent, uint itemId)
    {
        ref Countdown? countdown = ref CollectionsMarshal.GetValueRefOrAddDefault(idToCountdown, itemId, out _);
        if (countdown is null)
        {
            Avatar avatar = context.GetAvatar(itemId);
            countdown = new(avatar.GetOrCreateItem())
            {
                NameCardPic = avatar.NameCard is { PicturePrefix.Length: > 0 } nc
                    ? AvatarNameCardPicConverter.IconNameToUri(nc.PicturePrefix)
                    : null,
            };
            builder.Insert(0, countdown);
        }

        countdown.Histories.Add(new(gachaEvent));
    }

    private static void TrackWeaponItemId(GachaLogWishCountdownServiceMetadataContext context, Dictionary<uint, Countdown> idToCountdown, ImmutableArray<Countdown>.Builder builder, GachaEvent gachaEvent, uint itemId)
    {
        ref Countdown? countdown = ref CollectionsMarshal.GetValueRefOrAddDefault(idToCountdown, itemId, out _);
        if (countdown is null)
        {
            countdown = new(context.GetWeapon(itemId).GetOrCreateItem());
            builder.Insert(0, countdown);
        }

        countdown.Histories.Add(new(gachaEvent));
    }
}
