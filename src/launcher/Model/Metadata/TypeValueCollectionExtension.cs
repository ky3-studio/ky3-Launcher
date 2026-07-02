//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Model.Intrinsic;
using Launcher.ViewModel.Wiki;
using System.Collections.Immutable;

namespace Launcher.Model.Metadata;

internal static class TypeValueCollectionExtension
{
    extension(TypeValueCollection<FightProperty, GrowCurveType> collection)
    {
        public ImmutableArray<PropertyCurveValue> ToPropertyCurveValues(BaseValue baseValue)
        {
            return collection.Array.SelectAsArray(static (info, baseValue) => new PropertyCurveValue(info.Type, info.Value, baseValue), baseValue);
        }
    }
}
