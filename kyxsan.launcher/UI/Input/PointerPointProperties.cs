//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
using kyxsan.Win32.Foundation;
using kyxsan.Win32.System.SystemServices;

namespace kyxsan.UI.Input;

internal readonly struct PointerPointProperties
{
    public readonly int Delta;
    public readonly bool IsLeftButtonPressed;
    public readonly bool IsRightButtonPressed;
    public readonly bool IsShiftKeyPressed;
    public readonly bool IsControlKeyPressed;
    public readonly bool IsMiddleButtonPressed;
    public readonly bool IsXButton1Pressed;
    public readonly bool IsXButton2Pressed;
    public readonly POINT Point;

    public PointerPointProperties(int delta, MODIFIERKEYS_FLAGS flags, int x, int y)
    {
        Delta = delta;
        IsLeftButtonPressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_LBUTTON);
        IsRightButtonPressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_RBUTTON);
        IsShiftKeyPressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_SHIFT);
        IsControlKeyPressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_CONTROL);
        IsMiddleButtonPressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_MBUTTON);
        IsXButton1Pressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_XBUTTON1);
        IsXButton2Pressed = flags.HasFlag(MODIFIERKEYS_FLAGS.MK_XBUTTON2);
        Point = new(x, y);
    }
}