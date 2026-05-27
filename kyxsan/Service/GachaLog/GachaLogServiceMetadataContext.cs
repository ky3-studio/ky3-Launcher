//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using kyxsan.Model;
using kyxsan.Model.Intrinsic;
using kyxsan.Model.Metadata;
using kyxsan.Model.Metadata.Abstraction;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Model.Primitive;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableArray;
using kyxsan.Service.Metadata.ContextAbstraction.ImmutableDictionary;
using kyxsan.Service.Notification;
using kyxsan.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.Collections.Immutable;

namespace kyxsan.Service.GachaLog;

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

        throw kyxsanException.NotSupported($"Unsupported item type and name: '{type},{name}'");
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

        throw kyxsanException.NotSupported($"Unsupported item type and name: '{item.ItemType},{item.Name}'");
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
                catch
                {
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
                catch
                {
                }

                return new UnknownNameQuality($"Unknown Weapon ({id})", QualityType.QUALITY_NONE);
            }

            default:
            {
                try
                {
                    Ioc.Default.GetRequiredService<IMessenger>().Send(InfoBarMessage.Warning(SH.ServiceGachaLogIdPlacesUnsupported, $"{id} has places {place}"));
                }
                catch
                {
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
