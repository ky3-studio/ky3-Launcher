//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Metadata.Avatar;
using Launcher.Model.Metadata.Item;
using Launcher.Model.Primitive;
using System.Collections.Immutable;

namespace Launcher.ViewModel.Wiki;

internal sealed class CookBonusView
{
    public Material OriginItem { get; set; } = default!;

    public Material Item { get; set; } = default!;

    public static CookBonusView? Create(CookBonus? cookBonus, ImmutableDictionary<MaterialId, Material> idMaterialMap)
    {
        if (cookBonus is null)
        {
            return null;
        }

        // 元数据中可能存在未在 Material 字典中登记的占位 id（例如 114514 等），此处静默跳过以避免崩溃。
        if (!idMaterialMap.TryGetValue(cookBonus.OriginItemId, out Material? originItem))
        {
            return null;
        }

        if (!idMaterialMap.TryGetValue(cookBonus.ItemId, out Material? item))
        {
            return null;
        }

        CookBonusView view = new()
        {
            OriginItem = originItem,
            Item = item,
        };

        return view;
    }
}