//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.UI.Xaml.Data.Converter.Specialized;

[DependencyProperty<int>("Minimum", DefaultValue = 0, NotNull = true)]
[DependencyProperty<int>("Maximum", DefaultValue = int.MaxValue, NotNull = true)]
internal sealed partial class RangedInt32Converter : DependencyValueConverter<int, int>
{
    public override int Convert(int from)
    {
        return Math.Clamp(from, Minimum, Maximum);
    }

    public override int ConvertBack(int to)
    {
        return Math.Clamp(to, Minimum, Maximum);
    }
}