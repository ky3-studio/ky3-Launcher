//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Windows.UI;

namespace kyxsan.UI.Xaml.Data.Converter.Specialized;

[DependencyProperty<int>("MaximumValue", DefaultValue = 6, NotNull = true)]
[DependencyProperty<int>("MinimumValue", DefaultValue = 1, NotNull = true)]
[DependencyProperty<Color>("Maximum", NotNull = true)]
[DependencyProperty<Color>("Minimum", NotNull = true)]
internal sealed partial class UInt32ToGradientColorConverter : DependencyValueConverter<uint, Color>
{
    public UInt32ToGradientColorConverter()
    {
        Maximum = ColorHelper.ToColor(0xFFFD0093);
        Minimum = ColorHelper.ToColor(0xFF4B00D9);
    }

    public override Color Convert(uint from)
    {
        double n = Math.Clamp(from, MinimumValue, MaximumValue) - MinimumValue;
        int step = MaximumValue - MinimumValue;
        double a = Minimum.A + ((Maximum.A - Minimum.A) * n / step);
        double r = Minimum.R + ((Maximum.R - Minimum.R) * n / step);
        double g = Minimum.G + ((Maximum.G - Minimum.G) * n / step);
        double b = Minimum.B + ((Maximum.B - Minimum.B) * n / step);

        Unsafe.SkipInit(out Color color);
        color.A = (byte)a;
        color.R = (byte)r;
        color.G = (byte)g;
        color.B = (byte)b;
        return color;
    }
}