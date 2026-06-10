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

internal class DoubleTimeline
{
    private readonly TimelineProgressor progressor;

    public DoubleTimeline(double from = 0, double to = 1, double seconds = 1, TimeSpan? beginTime = null,
        bool autoReverse = true, bool forever = true, EasingFunctionBase? easingFunction = null)
    {
        progressor = new(seconds, autoReverse)
            { EasingFunction = easingFunction, BeginTime = beginTime, Forever = forever };
        From = from;
        To = to;
        Duration = new(TimeSpan.FromSeconds(seconds));
        AutoReverse = autoReverse;
        BeginTime = beginTime;
        Forever = forever;
    }

    public bool AutoReverse { get; }

    public TimeSpan? BeginTime { get; }

    public Duration Duration { get; }

    public bool Forever { get; }

    public double From { get; }

    public double To { get; }

    public double GetCurrentProgress(TimeSpan timeSpan)
    {
        double progress = progressor.GetCurrentProgress(timeSpan);
        return From + (To - From) * progress;
    }
}