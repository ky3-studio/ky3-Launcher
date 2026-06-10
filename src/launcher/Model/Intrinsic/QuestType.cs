//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model.Intrinsic;

internal enum QuestType : uint
{
    /// <summary>
    /// Archon Quest 魔神任务
    /// </summary>
    AQ,

    /// <summary>
    /// Fractions Quest 帮派任务
    /// </summary>
    FQ,

    /// <summary>
    /// Legend Quest 传说任务
    /// </summary>
    LQ,

    /// <summary>
    /// Event Quest 活动任务
    /// </summary>
    EQ,

    /// <summary>
    /// Daily Quest 日常任务
    /// </summary>
    DQ,

    /// <summary>
    /// Interval Quest 间隔任务?
    /// </summary>
    IQ,

    VQ,

    /// <summary>
    /// World Quest 世界任务
    /// </summary>
    WQ,
}