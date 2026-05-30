//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DependencyInjection.Abstraction;
using kyxsan.Model.Entity;
using kyxsan.Service.Abstraction;
using kyxsan.Service.User;
using kyxsan.ViewModel.DailyNote;
using kyxsan.ViewModel.User;
using kyxsan.Web.Hoyolab.Takumi.GameRecord;
using kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote;
using kyxsan.Web.Response;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using WebDailyNote = kyxsan.Web.Hoyolab.Takumi.GameRecord.DailyNote.DailyNote;

namespace kyxsan.Service.DailyNote;

[Service(ServiceLifetime.Singleton, typeof(IDailyNoteService))]
internal sealed partial class DailyNoteService : IDailyNoteService, IRecipient<UserRemovedMessage>
{
    private readonly DailyNoteNotificationOperation dailyNoteNotificationOperation;
    private readonly IDailyNoteRepository dailyNoteRepository;
    private readonly DailyNoteOptions dailyNoteOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    private readonly AsyncLock entriesLock = new();
    private ObservableCollection<DailyNoteEntry>? entries;

    [GeneratedConstructor]
    public partial DailyNoteService(IServiceProvider serviceProvider);

    public void Receive(UserRemovedMessage message)
    {
        // Database items have been deleted by cascade deleting.
        if (entries is { } localEntries)
        {
            Guid userId = message.RemovedUser.InnerId;
            taskContext.InvokeOnMainThread(() => localEntries.RemoveWhere(n => n.UserId == userId));
        }
    }

    public async ValueTask AddDailyNoteAsync(DailyNoteMetadataContext context, UserAndUid userAndUid, CancellationToken token = default)
    {
        string roleUid = userAndUid.Uid.Value;

        if (dailyNoteRepository.ContainsUid(roleUid))
        {
            return;
        }

        DailyNoteEntry newEntry = DailyNoteEntry.From(userAndUid);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            Response<WebDailyNote> dailyNoteResponse = await ScopedGetDailyNoteAsync(scope, userAndUid, token).ConfigureAwait(false);
            if (dailyNoteResponse.ReturnCode is not (int)KnownReturnCode.CODE5003
                && ResponseValidator.TryValidate(dailyNoteResponse, serviceProvider, out WebDailyNote? data))
            {
                await taskContext.SwitchToMainThreadAsync();
                newEntry.Update(data);
            }
        }

        dailyNoteRepository.AddDailyNoteEntry(newEntry);

        newEntry.UserGameRole = await userService.GetUserGameRoleByUidAsync(roleUid).ConfigureAwait(false);
        newEntry.ArchonQuestView = DailyNoteArchonQuestView.Create(newEntry.DailyNote, context.Chapters);

        // Set navigation property before adding to the database will cause a corresponding insert operation.
        // (Insert user to database, which is not expected)
        newEntry.User = userAndUid.User;

        await taskContext.SwitchToMainThreadAsync();
        entries?.Add(newEntry);
    }

    public async ValueTask<ObservableCollection<DailyNoteEntry>> GetDailyNoteEntryCollectionAsync(DailyNoteMetadataContext context, bool forceRefresh = false, CancellationToken token = default)
    {
        using (await entriesLock.LockAsync().ConfigureAwait(false))
        {
            if (entries is null)
            {
                ImmutableArray<DailyNoteEntry> entryList = dailyNoteRepository.GetDailyNoteEntryImmutableArrayIncludingUser();
                foreach (DailyNoteEntry entry in entryList)
                {
                    entry.UserGameRole = await userService.GetUserGameRoleByUidAsync(entry.Uid).ConfigureAwait(false);
                    entry.ArchonQuestView = DailyNoteArchonQuestView.Create(entry.DailyNote, context.Chapters);
                }

                entries = entryList.ToObservableCollection();

                await PrivateRefreshDailyNotesAsync(forceRefresh, token).ConfigureAwait(false);
            }

            return entries;
        }
    }

    public ValueTask RefreshDailyNotesAsync(CancellationToken token = default)
    {
        return PrivateRefreshDailyNotesAsync(true, token);
    }

    public async ValueTask RemoveDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await taskContext.SwitchToMainThreadAsync();
        ArgumentNullException.ThrowIfNull(entries);
        entries.Remove(entry);

        await taskContext.SwitchToBackgroundAsync();
        dailyNoteRepository.DeleteDailyNoteEntryById(entry.InnerId);
    }

    public async ValueTask<bool> UpdateDailyNoteAsync(DailyNoteEntry entry, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();
        string uid = entry.Uid;
        if (!dailyNoteRepository.Execute(query => query.Any(e => e.Uid == uid)))
        {
            return false;
        }

        dailyNoteRepository.UpdateDailyNoteEntry(entry);
        return true;
    }

    private static async ValueTask<Response<WebDailyNote>> ScopedGetDailyNoteAsync(IServiceScope scope, UserAndUid userAndUid, CancellationToken token = default)
    {
        IGameRecordClient gameRecordClient = scope.ServiceProvider
            .GetRequiredService<IOverseaSupportFactory<IGameRecordClient>>()
            .Create(userAndUid.IsOversea);

        Response<WebDailyNote> response = await gameRecordClient.GetDailyNoteAsync(userAndUid, token).ConfigureAwait(false);

        if (response.ReturnCode is (int)KnownReturnCode.CODE5003 or (int)KnownReturnCode.CODE1034)
        {
            CardClient cardClient = scope.ServiceProvider.GetRequiredService<CardClient>();
            Response<WidgetDailyNote> widgetResponse = await cardClient.GetWidgetDataAsync(userAndUid.User, token).ConfigureAwait(false);
            if (widgetResponse.ReturnCode is 0 && widgetResponse.Data is { } widgetData)
            {
                return new Response<WebDailyNote>(0, "OK", WebDailyNote.FromWidget(widgetData));
            }
        }

        return response;
    }

    private async ValueTask PrivateRefreshDailyNotesAsync(bool forceRefresh, CancellationToken token = default)
    {
        await taskContext.SwitchToBackgroundAsync();

        bool autoRefresh = dailyNoteOptions.IsAutoRefreshEnabled.Value;
        TimeSpan threshold = TimeSpan.FromSeconds(dailyNoteOptions.SelectedRefreshTime.Value?.Value ?? 60 * 60 * 4);

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            DailyNoteWebhookOperation dailyNoteWebhookOperation = serviceProvider.GetRequiredService<DailyNoteWebhookOperation>();
            foreach (DailyNoteEntry dbEntry in dailyNoteRepository.GetDailyNoteEntryImmutableArrayIncludingUser())
            {
                if (!(forceRefresh || (autoRefresh && dbEntry.RefreshTime < DateTimeOffset.Now - threshold)))
                {
                    continue;
                }

                Guid userId = dbEntry.UserId;
                string uid = dbEntry.Uid;
                if (entries?.SingleOrDefault(e => e.UserId == userId && e.Uid == uid) is not { } cachedEntry)
                {
                    // This can only happen when the entry is removing from the collection.
                    // And the entry is not removed from the database yet. We just skip it.
                    continue;
                }

                Response<WebDailyNote> dailyNoteResponse = await ScopedGetDailyNoteAsync(scope, UserAndUid.From(dbEntry.User, uid), token).ConfigureAwait(false);

                // 5003 (account risk) is a server-side flag that cannot be bypassed via API.
                // Silently skip and keep using cached data instead of showing error notifications.
                if (dailyNoteResponse.ReturnCode is (int)KnownReturnCode.CODE5003)
                {
                    continue;
                }

                if (!ResponseValidator.TryValidate(dailyNoteResponse, serviceProvider, out WebDailyNote? dailyNote))
                {
                    continue;
                }

                await taskContext.SwitchToMainThreadAsync();
                dbEntry.UserGameRole = cachedEntry.UserGameRole;
                dbEntry.Update(dailyNote);

                // The dbEntry will be updated before sending notification (Check suppression).
                await dailyNoteNotificationOperation.SendAsync(dbEntry).ConfigureAwait(false);
                dailyNoteRepository.UpdateDailyNoteEntry(dbEntry);

                dailyNoteWebhookOperation.TryPostDailyNoteToWebhook(uid, dailyNote);

                // After everything is done, we copy the updated entry to the cached entry.
                await taskContext.SwitchToMainThreadAsync();
                dbEntry.CopyTo(cachedEntry);
            }
        }
    }
}