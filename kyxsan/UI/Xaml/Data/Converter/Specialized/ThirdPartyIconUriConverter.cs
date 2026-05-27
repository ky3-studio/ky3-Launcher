//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media.Imaging;

namespace kyxsan.UI.Xaml.Data.Converter.Specialized;

internal sealed partial class ThirdPartyIconUriConverter : ValueConverter<string, BitmapImage>
{
    public override BitmapImage Convert(string from)
    {
        Uri uri = from switch
        {
            ThirdPartyIconConverter.TwitterName => "ms-appx:///Resource/ThirdParty/Twitter.png".ToUri(),
            _ => $"ms-appx:///Resource/ThirdParty/{from}.png".ToUri(),
        };
        
        return new BitmapImage(uri);
    }
}
