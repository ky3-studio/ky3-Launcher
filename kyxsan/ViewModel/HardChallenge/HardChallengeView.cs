//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model;
using kyxsan.Model.Entity;
using kyxsan.UI.Xaml.Data;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.HardChallenge;
using System.Collections.Immutable;
using MetadataHardChallengeSchedule = kyxsan.Model.Metadata.HardChallengeSchedule;

namespace kyxsan.ViewModel.HardChallenge;

internal sealed partial class HardChallengeView : IEntityAccess<HardChallengeEntry?>, IPropertyValuesProvider
{
    private HardChallengeView(HardChallengeEntry entity, HardChallengeMetadataContext context)
        : this(context.IdHardChallengeScheduleMap[entity.ScheduleId], context)
    {
        Entity = entity;

        HardChallengeData hardChallengeData = entity.HardChallengeData;

        ImmutableArray<DataEntryView>.Builder builder = ImmutableArray.CreateBuilder<DataEntryView>(2);

        if (DataEntryView.Create(true, hardChallengeData.SinglePlayer, context) is { } singlePlayer)
        {
            builder.Add(singlePlayer);
        }

        if (DataEntryView.Create(false, hardChallengeData.MultiPlayer, context) is { } multiPlayer)
        {
            builder.Add(multiPlayer);
        }

        DataEntries = builder.ToImmutable().AsAdvancedCollectionView();

        BlingAvatars = hardChallengeData.Blings.SelectAsArray(AvatarBling.Create, context);
        Engaged = true;
    }

    private HardChallengeView(MetadataHardChallengeSchedule hardChallengeSchedule, HardChallengeMetadataContext context)
    {
        ScheduleId = hardChallengeSchedule.Id;
        ScheduleName = hardChallengeSchedule.Name;
        FormattedTime = $"{hardChallengeSchedule.Begin:yyyy.MM.dd HH:mm} - {hardChallengeSchedule.End:yyyy.MM.dd HH:mm}";
    }

    public uint ScheduleId { get; }

    public string ScheduleName { get; }

    public string Schedule { get => SH.FormatModelEntityHardChallengeSchedule(ScheduleId - 5269000, ScheduleName); }

    public string FormattedTime { get; }

    public bool Engaged { get; }

    public HardChallengeEntry? Entity { get; }

    public IAdvancedCollectionView<DataEntryView>? DataEntries { get; }

    public ImmutableArray<AvatarBling> BlingAvatars { get; } = [];

    public static HardChallengeView Create(HardChallengeEntry entity, HardChallengeMetadataContext context)
    {
        return new(entity, context);
    }

    public static HardChallengeView Create(HardChallengeEntry? entity, MetadataHardChallengeSchedule meta, HardChallengeMetadataContext context)
    {
        return entity is not null ? new(entity, context) : new(meta, context);
    }
}