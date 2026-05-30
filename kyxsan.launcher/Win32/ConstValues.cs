//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Win32;

// ReSharper disable InconsistentNaming
[SuppressMessage("", "SA1310")]
internal static class ConstValues
{
    public const uint WM_ERASEBKGND = 0x00000014U;
    public const uint WM_NCLBUTTONDBLCLK = 0x000000A3U;
    public const uint WM_NCRBUTTONDOWN = 0x000000A4U;
    public const uint WM_NCRBUTTONUP = 0x000000A5U;
    public const uint WM_KEYDOWN = 0x00000100U;
    public const uint WM_KEYUP = 0x00000101U;
    public const uint WM_SYSKEYDOWN = 0x00000104U;
    public const uint WM_SYSKEYUP = 0x00000105U;
    public const uint WM_MOUSEWHEEL = 0x0000020AU;
}