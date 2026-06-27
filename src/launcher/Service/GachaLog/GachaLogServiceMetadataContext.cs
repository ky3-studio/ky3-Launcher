//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.ExceptionService;
using Launcher.Model;
using Launcher.Model.Intrinsic;
using Launcher.Model.Metadata;
using Launcher.Model.Metadata.Abstraction;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Weapon;
using Launcher.Model.Primitive;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableArray;
using Launcher.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using Launcher.Service.Notification;
using Launcher.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;

namespace Launcher.Service.GachaLog;

internal sealed class GachaLogServiceMetadataContext : IMetadataContext,
    IMetadataArrayGachaEventSource,
    IMetadataDictionaryIdAvatarSource,
    IMetadataDictionaryIdWeaponSource,
    IMetadataDictionaryNameAvatarSource,
    IMetadataDictionaryNameWeaponSource
{
    public ImmutableArray<GachaEvent> GachaEvents { get; set; } = default!;

    public ImmutableDictionary<AvatarId, Avatar> IdAvatarMap { get; set; } = default!;

    public ImmutableDictionary<WeaponId, Weapon> IdWeaponMap { get; set; } = default!;

    public ImmutableDictionary<string, Avatar> NameAvatarMap { get; set; } = default!;

    public ImmutableDictionary<string, Weapon> NameWeaponMap { get; set; } = default!;

    public Item GetItemByNameAndType(string name, string type)
    {
        if (string.Equals(type, SH.ModelInterchangeUIGFItemTypeAvatar, StringComparison.Ordinal))
        {
            return this.GetAvatar(name).GetOrCreateItem();
        }

        if (string.Equals(type, SH.ModelInterchangeUIGFItemTypeWeapon, StringComparison.Ordinal))
        {
            return this.GetWeapon(name).GetOrCreateItem();
        }

        throw LauncherException.NotSupported($"Unsupported item type and name: '{type},{name}'");
    }

    public uint GetItemId(GachaLogItem item)
    {
        if (string.Equals(item.ItemType, SH.ModelInterchangeUIGFItemTypeAvatar, StringComparison.Ordinal))
        {
            return this.GetAvatar(item.Name).Id;
        }

        if (string.Equals(item.ItemType, SH.ModelInterchangeUIGFItemTypeWeapon, StringComparison.Ordinal))
        {
            return this.GetWeapon(item.Name).Id;
        }

        throw LauncherException.NotSupported($"Unsupported item type and name: '{item.ItemType},{item.Name}'");
    }

    public INameQualityAccess GetNameQualityByItemId(uint id)
    {
        uint place = id.StringLength;
        switch (place)
        {
            case 8U:
                {
                    if (IdAvatarMap.TryGetValue(id, out Avatar? avatar))
                    {
                        return avatar;
                    }

                    try
                    {
                        Ioc.Default.GetRequiredService<IMessenger>().Send(InfoBarMessage.Warning(SH.ServiceGachaLogAvatarIdNotFound, $"{id}"));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                    return new UnknownNameQuality($"Unknown Avatar ({id})", QualityType.QUALITY_NONE);
                }

            case 5U:
                {
                    if (IdWeaponMap.TryGetValue(id, out Weapon? weapon))
                    {
                        return weapon;
                    }

                    try
                    {
                        Ioc.Default.GetRequiredService<IMessenger>().Send(InfoBarMessage.Warning(SH.ServiceGachaLogWeaponIdNotFound, $"{id}"));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                    return new UnknownNameQuality($"Unknown Weapon ({id})", QualityType.QUALITY_NONE);
                }

            default:
                {
                    try
                    {
                        Ioc.Default.GetRequiredService<IMessenger>().Send(InfoBarMessage.Warning(SH.ServiceGachaLogIdPlacesUnsupported, $"{id} has places {place}"));
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                    return new UnknownNameQuality($"Unknown ({id})", QualityType.QUALITY_NONE);
                }
        }
    }

    private sealed class UnknownNameQuality : INameQualityAccess
    {
        public UnknownNameQuality(string name, QualityType quality)
        {
            Name = name;
            Quality = quality;
        }

        public string Name { get; }

        public QualityType Quality { get; }
    }
}
