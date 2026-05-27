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

internal sealed partial class DailyNoteRefreshJob : IJob
{
    private readonly IDailyNoteService dailyNoteService;

    [GeneratedConstructor]
    public partial DailyNoteRefreshJob(IServiceProvider serviceProvider);

    [SuppressMessage("", "SH003")]
    public async Task Execute(IJobExecutionContext context)
    {
        await dailyNoteService.RefreshDailyNotesAsync(context.CancellationToken).ConfigureAwait(false);
    }
}