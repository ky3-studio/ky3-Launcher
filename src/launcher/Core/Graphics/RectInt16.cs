//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Windows.Graphics;

namespace kyxsan.Core.Graphics;

internal readonly struct RectInt16
{
    public readonly short X;
    public readonly short Y;
    public readonly short Width;
    public readonly short Height;

    private RectInt16(int x, int y, int width, int height)
    {
        X = (short)x;
        Y = (short)y;
        Width = (short)width;
        Height = (short)height;
    }

    public static implicit operator RectInt32(RectInt16 rect)
    {
        return new(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static implicit operator RectInt16(RectInt32 rect)
    {
        return new(rect.X, rect.Y, rect.Width, rect.Height);
    }

    public static unsafe explicit operator RectInt16(ulong value)
    {
        return *(RectInt16*)&value;
    }

    public static unsafe explicit operator ulong(RectInt16 rect)
    {
        return *(ulong*)&rect;
    }
}