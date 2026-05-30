//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Quartz;

namespace kyxsan.Service.Job;

[Service(ServiceLifetime.Transient, typeof(IJobScheduler))]
internal sealed class CookieTokenRefreshJobScheduler : IJobScheduler
{
    private const int RefreshIntervalSeconds = 43200;

    public async ValueTask ScheduleAsync(IScheduler scheduler)
    {
        IJobDetail job = JobBuilder.Create<CookieTokenRefreshJob>()
            .WithIdentity(JobIdentity.CookieTokenRefreshJobName, JobIdentity.CookieTokenGroupName)
            .Build();

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(JobIdentity.CookieTokenRefreshTriggerName, JobIdentity.CookieTokenGroupName)
            .WithSimpleSchedule(builder => builder.WithIntervalInSeconds(RefreshIntervalSeconds).RepeatForever())
            .StartAt(DateTimeOffset.Now.AddMinutes(5))
            .Build();

        await scheduler.ScheduleJob(job, trigger).ConfigureAwait(false);
    }
}
