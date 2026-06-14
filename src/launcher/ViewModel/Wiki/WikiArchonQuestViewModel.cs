//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Core.Logging;
using kyxsan.Model.Metadata.Converter;
using kyxsan.Model.Metadata.Quest;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Wiki;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiArchonQuestViewModel : Abstraction.ViewModel
{
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    public WikiArchonQuestViewModel(IServiceProvider serviceProvider)
    {
        metadataService = serviceProvider.GetRequiredService<IMetadataService>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();
    }

    public IAdvancedCollectionView<ArchonQuest>? Quests
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentQuestChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentQuestChanged);
        }
    }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    public uint TotalAdventureExp { get; set; }

    public uint TotalPrimogem { get; set; }

    public uint TotalMora { get; set; }

    public static Uri AdventureExpIconUri { get; } = ItemIconConverter.IconNameToUri("UI_ItemIcon_102");

    public static Uri PrimogemIconUri { get; } = ItemIconConverter.IconNameToUri("UI_ItemIcon_201");

    public static Uri MoraIconUri { get; } = ItemIconConverter.IconNameToUri("UI_ItemIcon_202");

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        WikiArchonQuestMetadataContext context = await metadataService.GetContextAsync<WikiArchonQuestMetadataContext>(token).ConfigureAwait(false);
        ImmutableArray<ArchonQuest> quests = context.ArchonQuests;
        SearchData searchData = SearchData.CreateForArchonQuest(quests);

        IAdvancedCollectionView<ArchonQuest> questsView;
        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            questsView = quests.AsAdvancedCollectionView();
        }

        await taskContext.SwitchToMainThreadAsync();
        token.ThrowIfCancellationRequested();

        (uint adventureExp, uint primogem, uint mora) = CalculateRewardTotals(quests);

        SearchData = searchData;
        Quests = questsView;
        Quests.MoveCurrentToFirst();
        TotalAdventureExp = adventureExp;
        TotalPrimogem = primogem;
        TotalMora = mora;

        return true;
    }

    private static (uint AdventureExp, uint Primogem, uint Mora) CalculateRewardTotals(ImmutableArray<ArchonQuest> quests)
    {
        uint adventureExp = 0;
        uint primogem = 0;
        uint mora = 0;

        foreach (ArchonQuest quest in quests)
        {
            foreach (ArchonQuestStory story in quest.Stories)
            {
                foreach (ArchonQuestReward reward in story.Rewards)
                {
                    switch (reward.Id)
                    {
                        case 102U:
                            adventureExp += reward.Count;
                            break;
                        case 201U:
                            primogem += reward.Count;
                            break;
                        case 202U:
                            mora += reward.Count;
                            break;
                    }
                }
            }
        }

        return (adventureExp, primogem, mora);
    }

    private void OnCurrentQuestChanged(object? sender, object? e)
    {
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter quests", "WikiArchonQuestViewModel.Command"));

        if (Quests is null)
        {
            return;
        }

        Quests.Filter = ArchonQuestFilter.Compile(SearchData);

        if (Quests.CurrentItem is null)
        {
            Quests.MoveCurrentToFirst();
        }
    }
}
