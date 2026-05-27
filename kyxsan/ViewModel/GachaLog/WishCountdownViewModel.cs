//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using kyxsan.Service.GachaLog;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.GachaLog;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class WishCountdownViewModel : Abstraction.ViewModel
{
    private readonly IGachaLogWishCountdownService gachaLogWishCountdownService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;

    [GeneratedConstructor]
    public partial WishCountdownViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial WishCountdownBundle? WishCountdowns { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial int SelectedCategoryIndex { get; set; }

    [ObservableProperty]
    public partial Countdown? SelectedCountdown { get; set; }

    public IList<Countdown>? CurrentCountdowns { get; private set => SetProperty(ref field, value); }

    public IList<Countdown>? FilteredOrangeAvatars { get; private set => SetProperty(ref field, value); }

    public IList<Countdown>? FilteredPurpleAvatars { get; private set => SetProperty(ref field, value); }

    public IList<Countdown>? FilteredOrangeWeapons { get; private set => SetProperty(ref field, value); }

    public IList<Countdown>? FilteredPurpleWeapons { get; private set => SetProperty(ref field, value); }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        GachaLogWishCountdownServiceMetadataContext context = await metadataService.GetContextAsync<GachaLogWishCountdownServiceMetadataContext>(token).ConfigureAwait(false);
        WishCountdownBundle countdowns = await gachaLogWishCountdownService.GetWishCountdownBundleAsync(context).ConfigureAwait(false);
        await taskContext.SwitchToMainThreadAsync();
        WishCountdowns = countdowns;
        return true;
    }

    partial void OnWishCountdownsChanged(WishCountdownBundle? value)
    {
        UpdateFilteredCountdowns(SearchText);
    }

    partial void OnSearchTextChanged(string value)
    {
        UpdateFilteredCountdowns(value);
    }

    partial void OnSelectedCategoryIndexChanged(int value)
    {
        UpdateCurrentCountdowns();
    }

    private void UpdateFilteredCountdowns(string? search)
    {
        if (WishCountdowns is null)
        {
            FilteredOrangeAvatars = null;
            FilteredPurpleAvatars = null;
            FilteredOrangeWeapons = null;
            FilteredPurpleWeapons = null;
            CurrentCountdowns = null;
            SelectedCountdown = null;
            return;
        }

        FilteredOrangeAvatars = FilterCountdowns(WishCountdowns.OrangeAvatars, search);
        FilteredPurpleAvatars = FilterCountdowns(WishCountdowns.PurpleAvatars, search);
        FilteredOrangeWeapons = FilterCountdowns(WishCountdowns.OrangeWeapons, search);
        FilteredPurpleWeapons = FilterCountdowns(WishCountdowns.PurpleWeapons, search);

        if (!string.IsNullOrWhiteSpace(search) && GetFilteredList(SelectedCategoryIndex) is not { Count: > 0 })
        {
            for (int i = 0; i < 4; i++)
            {
                if (GetFilteredList(i) is { Count: > 0 })
                {
                    SelectedCategoryIndex = i;
                    return;
                }
            }
        }

        UpdateCurrentCountdowns();
    }

    private void UpdateCurrentCountdowns()
    {
        CurrentCountdowns = GetFilteredList(SelectedCategoryIndex);
        SelectedCountdown = CurrentCountdowns?.FirstOrDefault();
    }

    private IList<Countdown>? GetFilteredList(int index)
    {
        return index switch
        {
            0 => FilteredOrangeAvatars,
            1 => FilteredPurpleAvatars,
            2 => FilteredOrangeWeapons,
            3 => FilteredPurpleWeapons,
            _ => null,
        };
    }

    private static IList<Countdown> FilterCountdowns(ImmutableArray<Countdown> source, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return source;
        }

        return [.. source.Where(c => c.Item.Name.Contains(search, StringComparison.OrdinalIgnoreCase))];
    }
}
