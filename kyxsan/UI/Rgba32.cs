//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Buffers.Binary;
using Windows.UI;

namespace kyxsan.UI;

internal struct Rgba32
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;

    public Rgba32(string hex)
        : this(hex.Length == 6 ? Convert.ToUInt32($"{hex}FF", 16) : Convert.ToUInt32(hex, 16))
    {
    }

    public unsafe Rgba32(uint xrgbaCode)
    {
        // uint layout: 0xRRGGBBAA is AABBGGRR
        // AABBGGRR -> RRGGBBAA
        fixed (Rgba32* pSelf = &this)
        {
            *(uint*)pSelf = BinaryPrimitives.ReverseEndianness(xrgbaCode);
        }
    }

    public Rgba32(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public readonly double Luminance { get => Luminance255 / 255; }

    public readonly double Luminance255 { get => (0.2126 * R) + (0.7152 * G) + (0.0722 * B); }

    public static implicit operator Color(Rgba32 rgba32)
    {
        return ColorHelper.ToColor(rgba32);
    }
}