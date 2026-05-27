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

internal static class SizeInt32Extension
{
    extension(SizeInt32 sizeInt32)
    {
        public int Size { get => sizeInt32.Width * sizeInt32.Height; }

        public SizeInt32 Scale(double scale)
        {
            return new((int)(sizeInt32.Width * scale), (int)(sizeInt32.Height * scale));
        }

        public unsafe RectInt32 ToRectInt32()
        {
            RectInt32View view = default;
            view.Size = sizeInt32;
            return *(RectInt32*)&view;
        }
    }
}