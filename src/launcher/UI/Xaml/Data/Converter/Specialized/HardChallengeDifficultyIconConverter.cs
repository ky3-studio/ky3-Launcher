//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Model.Intrinsic;

namespace kyxsan.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class HardChallengeDifficultyIconConverter : ValueConverter<HardChallengeDifficultyLevel, Uri>
{
    public static Uri Convert(string iconName)
    {
        return $"ms-appx:///Resource/Icon/{iconName}.png".ToUri();
    }

    public override Uri Convert(HardChallengeDifficultyLevel from)
    {
        return $"ms-appx:///Resource/Icon/UI_LeyLineChallenge_Medal_{from:D}.png".ToUri();
    }
}