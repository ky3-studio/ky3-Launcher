//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using kyxsan.Win32.UI.WindowsAndMessaging;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static kyxsan.Win32.ConstValues;

namespace kyxsan.UI.Input.LowLevel;

internal delegate void InputLowLevelKeyboardSourceEventHandler(LowLevelKeyEventArgs args);

internal static class InputLowLevelKeyboardSource
{
    public static event InputLowLevelKeyboardSourceEventHandler? KeyDown;

    public static event InputLowLevelKeyboardSourceEventHandler? KeyUp;

    public static event InputLowLevelKeyboardSourceEventHandler? SystemKeyDown;

    public static event InputLowLevelKeyboardSourceEventHandler? SystemKeyUp;

    [field: MaybeNull]
    private static kyxsanNativeInputLowLevelKeyboardSource Native
    {
        get => LazyInitializer.EnsureInitialized(ref field, kyxsanNative.Instance.MakeInputLowLevelKeyboardSource);
    }

    public static unsafe void Initialize()
    {
        Native.Attach(kyxsanNativeInputLowLevelKeyboardSourceCallback.Create(&ProcessLowLevelKeyboard));
    }

    public static unsafe void Uninitialize()
    {
        Native.Detach(kyxsanNativeInputLowLevelKeyboardSourceCallback.Create(&ProcessLowLevelKeyboard));
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe BOOL ProcessLowLevelKeyboard(uint param, KBDLLHOOKSTRUCT* data)
    {
        LowLevelKeyEventArgs args = new(*data);
        switch (param)
        {
            case WM_KEYDOWN:
                KeyDown?.Invoke(args);
                break;
            case WM_KEYUP:
                KeyUp?.Invoke(args);
                break;
            case WM_SYSKEYDOWN:
                SystemKeyDown?.Invoke(args);
                break;
            case WM_SYSKEYUP:
                SystemKeyUp?.Invoke(args);
                break;
        }

        return args.Handled;
    }
}