//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Quartz;
using kyxsan.Service.DailyNote;

namespace kyxsan.Service.Job;

[Service(ServiceLifetime.Transient, typeof(IJobScheduler))]
internal sealed partial class DailyNoteRefreshJobScheduler : IJobScheduler
{
    private readonly DailyNoteOptions dailyNoteOptions;

    [GeneratedConstructor]
    public partial DailyNoteRefreshJobScheduler(IServiceProvider serviceProvider);

    public async ValueTask ScheduleAsync(IScheduler scheduler)
    {
        if (!TryGetRefreshInterval(out int interval))
        {
            return;
        }

        IJobDetail dailyNoteJob = JobBuilder.Create<DailyNoteRefreshJob>()
            .WithIdentity(JobIdentity.DailyNoteRefreshJobName, JobIdentity.DailyNoteGroupName)
            .Build();

        ITrigger dailyNoteTrigger = TriggerBuilder.Create()
            .WithIdentity(JobIdentity.DailyNoteRefreshTriggerName, JobIdentity.DailyNoteGroupName)
            .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(interval).RepeatForever())
            .StartAt(DateTimeOffset.Now.AddSeconds(interval))
            .Build();

        await scheduler.ScheduleJob(dailyNoteJob, dailyNoteTrigger).ConfigureAwait(false);
    }

    private bool TryGetRefreshInterval(out int interval)
    {
        if (dailyNoteOptions is { IsAutoRefreshEnabled.Value: true, SelectedRefreshTime.Value: { } refreshTime })
        {
            interval = refreshTime.Value;
            return true;
        }

        interval = 0;
        return false;
    }
}