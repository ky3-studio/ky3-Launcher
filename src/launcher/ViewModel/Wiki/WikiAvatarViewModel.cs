//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Logging;
using Launcher.Model.Metadata;
using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Item;
using Launcher.Service.Launcher;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.UI.Xaml.Control.AutoSuggestBox;
using Launcher.UI.Xaml.Data;
using System.Collections.Immutable;

namespace Launcher.ViewModel.Wiki;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiAvatarViewModel : Abstraction.ViewModel
{
    private readonly ILauncherSpiralAbyssStatisticsCache LauncherCache;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    private WikiAvatarMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial WikiAvatarViewModel(IServiceProvider serviceProvider);

    public partial WikiAvatarStrategyComponent StrategyComponent { get; }

    public IAdvancedCollectionView<Avatar>? Avatars
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentAvatarChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentAvatarChanged);
        }
    }

    [ObservableProperty]
    public partial BaseValueInfo? BaseValueInfo { get; set; }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    [ObservableProperty]
    public partial LinkMetadataContext? LinkContext { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<WikiAvatarMetadataContext>(token).ConfigureAwait(false);
        ImmutableArray<Avatar> avatars = [.. metadataContext.Avatars.OrderByDescending(avatar => avatar.BeginTime).ThenByDescending(avatar => avatar.Sort)];
        SearchData searchData = SearchData.CreateForWikiAvatar(avatars);
        await CombineComplexDataAsync(avatars, metadataContext).ConfigureAwait(false);

        IAdvancedCollectionView<Avatar> avatarsView;
        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            avatarsView = avatars.AsAdvancedCollectionView();
        }

        await taskContext.SwitchToMainThreadAsync();
        token.ThrowIfCancellationRequested();

        SearchData = searchData;
        Avatars = avatarsView;
        Avatars.MoveCurrentToFirst();

        return true;
    }

    private void OnCurrentAvatarChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Avatars?.CurrentItem);
        UpdateLinkContext(Avatars?.CurrentItem);
        Avatars?.CurrentItem?.CostumesView?.MoveCurrentToFirst();
    }

    private async ValueTask CombineComplexDataAsync(ImmutableArray<Avatar> avatars, WikiAvatarMetadataContext context)
    {
        try
        {
            LauncherSpiralAbyssStatisticsMetadataContext context2 = await metadataService.GetContextAsync<LauncherSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
            await LauncherCache.InitializeForWikiAvatarViewAsync(context2).ConfigureAwait(false);

            if (LauncherCache.AvatarCollocations is { } collocations)
            {
                foreach (Avatar avatar in avatars)
                {
                    avatar.CollocationView = collocations.GetValueOrDefault(avatar.Id);
                }
            }
        }
        catch
        {
        }

        foreach (Avatar avatar in avatars)
        {
            avatar.CookBonusView ??= CookBonusView.Create(avatar.FetterInfo.CookBonus, context.IdMaterialMap);
            avatar.CultivationItemsView ??= [.. avatar.CultivationItems.Select(i => context.IdMaterialMap.GetValueOrDefault(i, Material.Default))];
            avatar.CostumesView ??= avatar.Costumes.OrderByDescending(c => c.IsDefault).AsAdvancedCollectionView();
        }
    }

    private void UpdateBaseValueInfo(Avatar? avatar)
    {
        if (avatar is null || metadataContext is null)
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfoMetadataContext context = new()
        {
            GrowCurveMap = metadataContext.LevelDictionaryAvatarGrowCurveMap,
            PromoteMap = metadataContext.IdDictionaryAvatarLevelPromoteMap[avatar.PromoteId],
        };

        BaseValueInfo = new(avatar.MaxLevel, avatar.GrowCurves.ToPropertyCurveValues(avatar.BaseValue), context);
    }

    private void UpdateLinkContext(Avatar? avatar)
    {
        if (avatar is null || metadataContext is null)
        {
            LinkContext = null;
            return;
        }

        LinkContext = new()
        {
            IdNameMap = metadataContext.IdHyperLinkNameMap,
            Inherents = avatar.SkillDepot.Inherents,
            Skills = avatar.SkillDepot.CompositeSkillsNoInherents,
        };
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter avatars", "WikiAvatarViewModel.Command"));

        if (Avatars is null)
        {
            return;
        }

        Avatars.Filter = AvatarFilter.Compile(SearchData);

        if (Avatars.CurrentItem is null)
        {
            Avatars.MoveCurrentToFirst();
        }
    }
}