//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using kyxsan.UI.Input;
using kyxsan.UI.Windowing.Abstraction;
using kyxsan.UI.Xaml.Media.Backdrop;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;
using kyxsan.Win32.System.SystemServices;
using kyxsan.Win32.UI.Shell;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static kyxsan.Win32.ConstValues;

namespace kyxsan.UI.Windowing;

internal sealed partial class XamlWindowSubclass : IDisposable
{
    private readonly kyxsanNativeWindowSubclass native;
    private readonly Window window;
    private GCHandle<XamlWindowSubclass> handle;

    private readonly Lock syncRoot = new();
    private bool taskBarInitialized;

    public unsafe XamlWindowSubclass(Window window)
    {
        this.window = window;
        handle = new(this);
        kyxsanNativeWindowSubclassCallback callback = kyxsanNativeWindowSubclassCallback.Create(&OnSubclassProcedure);
        native = kyxsanNative.Instance.MakeWindowSubclass(window.GetWindowHandle(), callback, handle);
    }

    public void Initialize()
    {
        native.Attach();
    }

    public void Dispose()
    {
        try
        {
            native.Detach();
        }
        catch (COMException)
        {
            // 0x80004005 E_FAIL
        }

        handle.Dispose();
    }

    public void SetTaskbarProgress(TBPFLAG state, ulong value, ulong maximum)
    {
        lock (syncRoot)
        {
            if (!taskBarInitialized)
            {
                native.InitializeTaskbarProgress();
                taskBarInitialized = true;
            }

            native.SetTaskbarProgress(state, value, maximum);
        }
    }

    /// <returns>Whether to call DefSubclassProc</returns>
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe BOOL OnSubclassProcedure(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam, GCHandle<XamlWindowSubclass> access, LRESULT* result)
    {
        if (XamlApplicationLifetime.Exiting)
        {
            return BOOL.FALSE;
        }

        XamlWindowSubclass? state = access.Target;
        ArgumentNullException.ThrowIfNull(state);

        switch (uMsg)
        {
            case WM_NCRBUTTONDOWN:
            case WM_NCRBUTTONUP:
                return BOOL.FALSE;

            case WM_NCLBUTTONDBLCLK:
                {
                    if (state.window.AppWindow.Presenter is OverlappedPresenter { IsMaximizable: false })
                    {
                        return BOOL.FALSE;
                    }

                    break;
                }

            case WM_ERASEBKGND:
                {
                    if (state.window is IWindowNeedEraseBackground ||
                        state.window.SystemBackdrop is IBackdropNeedEraseBackground)
                    {
                        *result = BOOL.TRUE;
                        return BOOL.FALSE;
                    }

                    break;
                }

            case WM_MOUSEWHEEL:
                {
                    if (state.window is IXamlWindowMouseWheelHandler handler)
                    {
                        WPARAM2MOUSEWHEEL pWParam2 = *(WPARAM2MOUSEWHEEL*)&wParam;
                        LPARAM2MOUSEWHEEL pLParam2 = *(LPARAM2MOUSEWHEEL*)&lParam;
                        PointerPointProperties data = new(pWParam2.High, (MODIFIERKEYS_FLAGS)pWParam2.Low, pLParam2.Low, pLParam2.High);
                        handler.OnMouseWheel(in data);
                    }

                    break;
                }
        }

        return BOOL.TRUE;
    }

#pragma warning disable CA1823
#pragma warning disable CS0169
#pragma warning disable CS0649

    // ReSharper disable once InconsistentNaming
    private readonly struct WPARAM2MOUSEWHEEL
    {
        public readonly short Low;
        public readonly short High;
        private readonly int reserved;
    }

    // ReSharper disable once InconsistentNaming
    private readonly struct LPARAM2MOUSEWHEEL
    {
        public readonly short Low;
        public readonly short High;
        private readonly int reserved;
    }

#pragma warning restore CS0649
#pragma warning restore CS0169
#pragma warning restore CA1823
}