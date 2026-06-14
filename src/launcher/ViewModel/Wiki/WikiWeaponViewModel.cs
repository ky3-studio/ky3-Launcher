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
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.UI.Xaml.Control.AutoSuggestBox;
using kyxsan.UI.Xaml.Data;
using System.Collections.Immutable;
using System.Net.Http;

namespace kyxsan.ViewModel.Wiki;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WikiWeaponViewModel : Abstraction.ViewModel
{
    private readonly IkyxsanSpiralAbyssStatisticsCache kyxsanCache;
    private readonly IMetadataService metadataService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ITaskContext taskContext;

    private WikiWeaponMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial WikiWeaponViewModel(IServiceProvider serviceProvider);

    public IAdvancedCollectionView<Weapon>? Weapons
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentWeaponChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentWeaponChanged);
        }
    }

    [ObservableProperty]
    public partial BaseValueInfo? BaseValueInfo { get; set; }

    [ObservableProperty]
    public partial SearchData? SearchData { get; set; }

    [ObservableProperty]
    public partial string? WeaponStory { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        metadataContext = await metadataService.GetContextAsync<WikiWeaponMetadataContext>(token).ConfigureAwait(false);
        ImmutableArray<Weapon> weapons = [.. metadataContext.Weapons.OrderByDescending(weapon => weapon.Sort)];
        SearchData searchData = SearchData.CreateForWikiWeapon(weapons);

        await CombineComplexDataAsync(weapons, metadataContext).ConfigureAwait(false);

        IAdvancedCollectionView<Weapon> weaponsView;
        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            weaponsView = weapons.AsAdvancedCollectionView();
        }

        await taskContext.SwitchToMainThreadAsync();
        token.ThrowIfCancellationRequested();

        SearchData = searchData;
        Weapons = weaponsView;
        Weapons.MoveCurrentToFirst();

        return true;
    }

    private void OnCurrentWeaponChanged(object? sender, object? e)
    {
        UpdateBaseValueInfo(Weapons?.CurrentItem);
        _ = FetchWeaponStoryAsync(Weapons?.CurrentItem);
    }

    private async Task FetchWeaponStoryAsync(Weapon? weapon)
    {
        if (weapon is null)
        {
            WeaponStory = null;
            return;
        }

        uint weaponId = (uint)weapon.Id;

        try
        {
            using HttpClient client = httpClientFactory.CreateClient();
            string url = $"https://gi.yatta.moe/api/v2/chs/readable/Weapon{weaponId}";
            string json = await client.GetStringAsync(url).ConfigureAwait(false);

            JsonDocument doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("data", out JsonElement dataElement))
            {
                string? story = dataElement.GetString();
                await taskContext.SwitchToMainThreadAsync();
                WeaponStory = string.IsNullOrWhiteSpace(story) ? null : story.Trim();
            }
            else
            {
                await taskContext.SwitchToMainThreadAsync();
                WeaponStory = null;
            }
        }
        catch
        {
            await taskContext.SwitchToMainThreadAsync();
            WeaponStory = null;
        }
    }

    private async ValueTask CombineComplexDataAsync(ImmutableArray<Weapon> weapons, WikiWeaponMetadataContext context)
    {
        try
        {
            kyxsanSpiralAbyssStatisticsMetadataContext context2 = await metadataService.GetContextAsync<kyxsanSpiralAbyssStatisticsMetadataContext>().ConfigureAwait(false);
            await kyxsanCache.InitializeForWikiWeaponViewAsync(context2).ConfigureAwait(false);

            if (kyxsanCache.WeaponCollocations is { } collocations)
            {
                foreach (Weapon weapon in weapons)
                {
                    weapon.CollocationView = collocations.GetValueOrDefault(weapon.Id);
                }
            }
        }
        catch
        {
        }

        foreach (Weapon weapon in weapons)
        {
            weapon.CultivationItemsView ??= [.. weapon.CultivationItems.Select(i => context.IdMaterialMap.GetValueOrDefault(i, Material.Default))];
        }
    }

    private void UpdateBaseValueInfo(Weapon? weapon)
    {
        if (weapon is null || metadataContext is null)
        {
            BaseValueInfo = null;
            return;
        }

        BaseValueInfoMetadataContext context = new()
        {
            GrowCurveMap = metadataContext.LevelDictionaryWeaponGrowCurveMap,
            PromoteMap = metadataContext.IdDictionaryWeaponLevelPromoteMap[weapon.PromoteId],
        };

        BaseValueInfo = new(weapon.MaxLevel, weapon.GrowCurves.ToPropertyCurveValues(), context);
    }

    [Command("FilterCommand")]
    private void ApplyFilter()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Filter weapons", "WikiWeaponViewModel.Command"));

        if (Weapons is null)
        {
            return;
        }

        Weapons.Filter = WeaponFilter.Compile(SearchData);

        if (Weapons.CurrentItem is null)
        {
            Weapons.MoveCurrentToFirst();
        }
    }
}
