//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Media;
using Launcher.Model.Intrinsic;
using Launcher.UI.Xaml.Data.Converter;

namespace Launcher.Model.Metadata.Converter;

[DependencyProperty<ImageSource>("RedSource")]
[DependencyProperty<ImageSource>("OrangeSource")]
[DependencyProperty<ImageSource>("PurpleSource")]
[DependencyProperty<ImageSource>("BlueSource")]
[DependencyProperty<ImageSource>("GreenSource")]
[DependencyProperty<ImageSource>("WhiteSource")]
[DependencyProperty<ImageSource>("NoneSource")]
internal sealed partial class QualityToImageSourceConverter : DependencyValueConverter<QualityType, ImageSource?>
{
    public override ImageSource? Convert(QualityType from)
    {
        return from switch
        {
            QualityType.QUALITY_ORANGE_SP => RedSource,
            QualityType.QUALITY_ORANGE => OrangeSource,
            QualityType.QUALITY_PURPLE => PurpleSource,
            QualityType.QUALITY_BLUE => BlueSource,
            QualityType.QUALITY_GREEN => GreenSource,
            QualityType.QUALITY_WHITE => WhiteSource,
            _ => NoneSource,
        };
    }
}