//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics;

namespace kyxsan.Core.Graphics;

internal static class RectInt32Convert
{
    public static RectInt32 RectInt32(RECT rect)
    {
        return new(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
    }

    public static RectInt32 RectInt32(Point position, Vector2 size)
    {
        return new((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
    }

    public static RectInt32 RectInt32(int x, int y, Vector2 size)
    {
        return new(x, y, (int)size.X, (int)size.Y);
    }

    public static unsafe RectInt32 RectInt32(PointInt32 position, SizeInt32 size)
    {
        RectInt32View view = default;
        view.Position = position;
        view.Size = size;
        return *(RectInt32*)&view;
    }
}