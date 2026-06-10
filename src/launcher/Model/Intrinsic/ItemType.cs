//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Model.Intrinsic;

internal enum ItemType
{
    /// <summary>
    /// 无
    /// </summary>
    ITEM_NONE = 0,

    /// <summary>
    /// 虚拟道具
    /// </summary>
    ITEM_VIRTUAL = 1,

    /// <summary>
    /// 材料
    /// </summary>
    ITEM_MATERIAL = 2,

    /// <summary>
    /// 圣遗物
    /// </summary>
    ITEM_RELIQUARY = 3,

    /// <summary>
    /// 武器
    /// </summary>
    ITEM_WEAPON = 4,

    /// <summary>
    /// 任务等
    /// </summary>
    ITEM_DISPLAY = 5,

    /// <summary>
    /// 家具
    /// </summary>
    ITEM_FURNITURE = 6,
}