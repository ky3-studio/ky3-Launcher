//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using kyxsan.Core.Setting;
using kyxsan.Model.Metadata.Avatar;
using kyxsan.Model.Metadata.Item;
using kyxsan.Model.Metadata.Weapon;
using kyxsan.Model.Primitive;
using kyxsan.Service;
using kyxsan.Service.Announcement;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.UI.Xaml.Data;
using kyxsan.ViewModel.Calendar;
using kyxsan.Web.Hoyolab.Hk4e.Common.Announcement;
using kyxsan.Web.WebView2;
using System.Collections.Immutable;

namespace kyxsan.ViewModel.Home;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class AnnouncementViewModel : Abstraction.ViewModel
{
    private readonly IAnnouncementService announcementService;
    private readonly IMetadataService metadataService;
    private readonly IServiceProvider serviceProvider;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;

    private DispatcherTimer? timeRefreshTimer;

    [GeneratedConstructor]
    public partial AnnouncementViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial AnnouncementWrapper? Announcement { get; set; }

    [ObservableProperty]
    public partial string GreetingText { get; set; } = SH.ViewPageHomeGreetingTextDefault;

    [ObservableProperty]
    public partial IAdvancedCollectionView<CalendarDay>? WeekDays { get; set; }

    protected override ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        _ = CoreWebView2EnvironmentFactory.GetAsync();
        InitializeInGameAnnouncementAsync(token).SafeForget();
        InitializeCalendarAsync(token).SafeForget();
        UpdateGreetingText();
        return ValueTask.FromResult(true);
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeInGameAnnouncementAsync(CancellationToken token)
    {
        try
        {
            AnnouncementWrapper? announcementWrapper = await announcementService.GetAnnouncementWrapperAsync(cultureOptions.LanguageCode, appOptions.Region.Value, token).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Announcement = announcementWrapper;
            DeferContentLoader?.Load("GameAnnouncementPivot");
            StartTimeRefreshTimer();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void UpdateGreetingText()
    {
        int rand = Random.Shared.Next(0, 1000);

        if (rand is >= 0 and < 6)
        {
            GreetingText = SH.ViewPageHomeGreetingTextEasterEgg;
        }
        else if (rand is >= 6 and < 57)
        {
            GreetingText = SH.FormatViewPageHomeGreetingTextCommon2(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
        }
        else if (rand is >= 57 and < 1000)
        {
            GreetingText = SH.FormatViewPageHomeGreetingTextCommon2(LocalSetting.Get(SettingKeys.LaunchTimes, 0));
        }
    }

    private void StartTimeRefreshTimer()
    {
        if (timeRefreshTimer is not null)
        {
            return;
        }

        timeRefreshTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timeRefreshTimer.Tick += OnTimeRefreshTimerTick;
        timeRefreshTimer.Start();
    }

    private void OnTimeRefreshTimerTick(object? sender, object e)
    {
        if (Announcement?.List is not { } wrappers)
        {
            return;
        }

        foreach (AnnouncementListWrapper wrapper in wrappers)
        {
            foreach (Announcement item in wrapper.List)
            {
                if (item.StartTime != default && item.EndTime != default)
                {
                    item.NotifyTimePropertiesChanged();
                }
            }
        }
    }

    [SuppressMessage("", "SH003")]
    private async Task InitializeCalendarAsync(CancellationToken token)
    {
        CalendarMetadataContext context = await metadataService.GetContextAsync<CalendarMetadataContext>(token).ConfigureAwait(false);

        // Build avatar birthday lookup
        ILookup<MonthAndDay, Avatar> avatarBirthdays = context.Avatars
            .Where(a => a.FetterInfo is not null && a.FetterInfo.BirthMonth > 0)
            .ToLookup(a => MonthAndDay.Create(a));

        // Build material → items lookup (avatars and weapons that use each material)
        Dictionary<MaterialId, List<CalendarItem>> materialItemsDict = [];

        foreach (Avatar avatar in context.Avatars)
        {
            foreach (MaterialId materialId in avatar.CultivationItems)
            {
                if (!materialItemsDict.TryGetValue(materialId, out List<CalendarItem>? list))
                {
                    list = [];
                    materialItemsDict[materialId] = list;
                }

                list.Add(avatar.ToItem<CalendarItem>());
            }
        }

        foreach (Weapon weapon in context.Weapons)
        {
            if (weapon.Quality < Model.Intrinsic.QualityType.QUALITY_BLUE)
            {
                continue;
            }

            foreach (MaterialId materialId in weapon.CultivationItems)
            {
                if (!materialItemsDict.TryGetValue(materialId, out List<CalendarItem>? list))
                {
                    list = [];
                    materialItemsDict[materialId] = list;
                }

                list.Add(weapon.ToItem<CalendarItem>());
            }
        }

        ILookup<MaterialId, CalendarItem> materialItems = materialItemsDict
            .SelectMany(kvp => kvp.Value.Select(item => (kvp.Key, item)))
            .ToLookup(x => x.Key, x => x.item);

        // Get server time
        TimeSpan offset = appOptions.CalendarServerTimeZoneOffset.Value;
        DateTimeOffset serverNow = DateTimeOffset.Now.ToOffset(offset);

        // Adjust for 4AM server reset (day starts at 4:00 AM)
        DateTimeOffset adjustedNow = serverNow - TimeSpan.FromHours(4);

        // Build 7 days starting from Monday of the current week
        DayOfWeek currentDay = adjustedNow.DayOfWeek;
        int daysToMonday = ((int)currentDay - (int)DayOfWeek.Monday + 7) % 7;
        DateTimeOffset monday = adjustedNow.Date.AddDays(-daysToMonday);

        List<CalendarDay> weekDays = new(7);

        for (int i = 0; i < 7; i++)
        {
            DateTimeOffset dayDate = monday.AddDays(i);
            DayOfWeek dow = dayDate.DayOfWeek;

            // Get rotational material entries for this day
            IEnumerable<RotationalMaterialIdEntry> entries = dow switch
            {
                DayOfWeek.Monday or DayOfWeek.Thursday => MaterialIds.MondayThursdayEntries,
                DayOfWeek.Tuesday or DayOfWeek.Friday => MaterialIds.TuesdayFridayEntries,
                DayOfWeek.Wednesday or DayOfWeek.Saturday => MaterialIds.WednesdaySaturdayEntries,
                _ => MaterialIds.MondayThursdayEntries.Concat(MaterialIds.TuesdayFridayEntries).Concat(MaterialIds.WednesdaySaturdayEntries), // Sunday: all
            };

            // Build CalendarMaterial list
            ImmutableArray<CalendarMaterial> materials = [.. entries.Select(entry =>
            {
                Material material = context.IdMaterialMap.GetValueOrDefault(entry.Highest, Material.Default);
                ImmutableArray<CalendarItem> items = [.. materialItems[entry.Highest]];
                return new CalendarMaterial
                {
                    Inner = material,
                    Items = items,
                    InnerEntry = entry,
                };
            })];

            // Get birthday avatars
            MonthAndDay md = new((uint)dayDate.Month, (uint)dayDate.Day);
            ImmutableArray<Model.Item> birthDayAvatars = [.. avatarBirthdays[md].Select(a => a.ToItem<Model.Item>())];

            weekDays.Add(new CalendarDay
            {
                Date = dayDate,
                DayInMonth = dayDate.Day,
                DayName = cultureOptions.CurrentCulture.Value.DateTimeFormat.GetAbbreviatedDayName(dow),
                BirthDayAvatars = birthDayAvatars,
                Materials = materials,
            });
        }

        await taskContext.SwitchToMainThreadAsync();

        IAdvancedCollectionView<CalendarDay> view = weekDays.AsAdvancedCollectionView();

        // Select the current day
        int todayIndex = daysToMonday;
        if (todayIndex >= 0 && todayIndex < 7)
        {
            view.MoveCurrentTo(weekDays[todayIndex]);
        }

        WeekDays = view;
    }

    protected override void UninitializeOverride()
    {
        if (timeRefreshTimer is not null)
        {
            timeRefreshTimer.Stop();
            timeRefreshTimer.Tick -= OnTimeRefreshTimerTick;
            timeRefreshTimer = null;
        }
    }
}