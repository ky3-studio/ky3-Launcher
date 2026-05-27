//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.UI.Xaml.Data;
using kyxsan.Web.Hoyolab.Takumi.Event.BbsSignReward;

namespace kyxsan.ViewModel.Sign;

internal sealed partial class AwardView : IPropertyValuesProvider
{
    public required int Index { get; init; }

    public required string Icon { get; init; }

    public required string Name { get; init; }

    public required int Count { get; init; }

    public bool IsClaimed { get; set; }

    public static AwardView Create(Award award, int index)
    {
        return new()
        {
            Index = index + 1,
            Icon = award.Icon,
            Name = award.Name,
            Count = award.Count,
        };
    }
}