//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.ViewModel.HardChallenge;
using kyxsan.ViewModel.User;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace kyxsan.Service.HardChallenge;

internal interface IHardChallengeService
{
    ValueTask<ObservableCollection<HardChallengeView>> GetHardChallengeViewCollectionAsync(HardChallengeMetadataContext context, UserAndUid userAndUid);

    ValueTask<ImmutableArray<AvatarView>> GetBlingAvatarsAsync(HardChallengeMetadataContext context, UserAndUid userAndUid);

    ValueTask RefreshHardChallengeAsync(HardChallengeMetadataContext context, UserAndUid userAndUid);
}