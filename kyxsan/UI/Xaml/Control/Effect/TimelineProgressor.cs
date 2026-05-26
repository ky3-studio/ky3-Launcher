//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace kyxsan.UI.Xaml.Control.Effect;

internal sealed class TimelineProgressor
{
    public TimelineProgressor(double seconds, bool autoReverse)
    {
        Duration = new(TimeSpan.FromSeconds(seconds));
        AutoReverse = autoReverse;
    }

    public bool AutoReverse { get; set; }

    public TimeSpan? BeginTime { get; set; }

    public Duration Duration { get; set; }

    public EasingFunctionBase? EasingFunction { get; set; }

    public bool Forever { get; set; }

    public double GetCurrentProgress(TimeSpan timeSpan)
    {
        long beginTimeTicks = 0L;

        if (BeginTime != null)
        {
            beginTimeTicks = BeginTime.Value.Ticks;
        }

        if (timeSpan.Ticks <= beginTimeTicks)
        {
            return 0;
        }

        long durationTicks = Duration.TimeSpan.Ticks;
        double scalingFactor = AutoReverse ? 2D : 1D;
        if (timeSpan.Ticks - beginTimeTicks > durationTicks * scalingFactor && !Forever)
        {
            return 0;
        }

        double offsetFromBegin = (timeSpan.Ticks - beginTimeTicks) % (durationTicks * scalingFactor);

        if (offsetFromBegin > durationTicks)
        {
            offsetFromBegin = durationTicks * 2 - offsetFromBegin;
        }

        double progress = offsetFromBegin / durationTicks;

        if (EasingFunction is not null)
        {
            progress = EasingFunction.Ease(progress);
        }

        return progress;
    }
}