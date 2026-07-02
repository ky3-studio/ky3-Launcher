//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Launcher.Core;
using Launcher.Core.LifeCycle;
using Launcher.Core.Setting;
using Launcher.Factory.ContentDialog;
using Launcher.UI.Windowing;
using Launcher.UI.Xaml.View.Window;
using Launcher.ViewModel.Guide;
using Launcher.Win32;
using Launcher.Win32.Foundation;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Launcher.UI.Shell;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class NotifyIconController : IDisposable
{
    private static bool constructed;

    private readonly Lock syncRoot = new();

    private readonly ICurrentXamlWindowReference currentXamlWindowReference;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly LazySlim<NotifyIconContextMenu> lazyMenu;
    private readonly NotifyIconXamlHostWindow xamlHostWindow;
    private readonly IServiceProvider serviceProvider;
    private readonly LauncherNativeNotifyIcon native;
    private GCHandle<NotifyIconController> handle;

    private bool disposed;

    public NotifyIconController(IServiceProvider serviceProvider)
    {
        if (Interlocked.Exchange(ref constructed, true))
        {
            // Actively prevent multiple constructions, if this happens, it's definitely a bug.
            // For example: the below part of the ctor throws an exception.
            throw new InvalidOperationException("NotifyIconController is already constructed.");
        }

        currentXamlWindowReference = serviceProvider.GetRequiredService<ICurrentXamlWindowReference>();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        this.serviceProvider = serviceProvider;
        lazyMenu = new(() => new(serviceProvider));

        string iconPath = InstalledLocation.GetAbsolutePath("Assets/Logo.ico");
        Guid id = MemoryMarshal.AsRef<Guid>(MD5.HashData(Encoding.UTF8.GetBytes(iconPath)).AsSpan());
        native = LauncherNative.Instance.MakeNotifyIcon(iconPath, in id);

        xamlHostWindow = new(serviceProvider);
        xamlHostWindow.MoveAndResize(default);

        handle = new(this);
    }

    public static Lock InitializationSyncRoot { get; } = new();

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        lock (syncRoot)
        {
            disposed = true;
            try
            {
                native.Destroy();
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

            handle.Dispose();
        }
    }

    public unsafe void Create()
    {
        native.Create(LauncherNativeNotifyIconCallback.Create(&OnNotifyIconCallback), handle, "ky3 Launcher");
    }

    public bool IsPromoted()
    {
        ObjectDisposedException.ThrowIf(disposed, this);
        try
        {
            return native.IsPromoted;
        }
        catch (Exception ex)
        {
            // If the lpValue registry value does not exist, the function returns ERROR_FILE_NOT_FOUND
            if (ex is not (FileNotFoundException or COMException or ObjectDisposedException))
            {
                SentrySdk.CaptureException(ex);
            }

            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static void OnNotifyIconCallback(LauncherNativeNotifyIconCallbackKind kind, RECT icon, POINT point, GCHandle<NotifyIconController> data)
    {
        if (data.Target is not { } controller)
        {
            return;
        }

        switch (kind)
        {
            case LauncherNativeNotifyIconCallbackKind.TaskbarCreated:
                controller.OnRecreateNotifyIconRequested();
                break;
            case LauncherNativeNotifyIconCallbackKind.ContextMenu:
                controller.OnContextMenuRequested(icon, point);
                break;
            case LauncherNativeNotifyIconCallbackKind.LeftButtonDown:
                controller.OnWindowRequested();
                break;
            case LauncherNativeNotifyIconCallbackKind.LeftButtonDoubleClick:
                controller.OnWindowRequested();
                break;
        }
    }

    private void OnRecreateNotifyIconRequested()
    {
        if (disposed || XamlApplicationLifetime.Exiting)
        {
            return;
        }

        native.Recreate("ky3 Launcher");
    }

    private void OnContextMenuRequested(RECT icon, POINT point)
    {
        if (disposed)
        {
            return;
        }

        if (XamlApplicationLifetime.Exiting)
        {
            Debugger.Break();
            return;
        }

        // https://github.com/DGP-Studio/Launcher/issues/2434
        // Now we disable the context menu when the dialog is showing.
        if (contentDialogFactory.IsDialogShowing)
        {
            return;
        }

        xamlHostWindow.ShowFlyoutAt(lazyMenu.Value, new(point.x, point.y), icon);
    }

    private void OnWindowRequested()
    {
        if (disposed)
        {
            return;
        }

        if (XamlApplicationLifetime.Exiting)
        {
            Debugger.Break();
            return;
        }

        switch (currentXamlWindowReference.Window)
        {
            case null:
                {
                    GuideState guideState = UnsafeLocalSetting.Get(SettingKeys.GuideState, GuideState.Language);
                    if (guideState < GuideState.Completed)
                    {
                        GuideWindow guideWindow = serviceProvider.GetRequiredService<GuideWindow>();
                        currentXamlWindowReference.Window = guideWindow;
                        guideWindow.SwitchTo();
                        guideWindow.AppWindow.MoveInZOrderAtTop();
                    }
                    else
                    {
                        MainWindow mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                        currentXamlWindowReference.Window = mainWindow;
                        mainWindow.SwitchTo();
                        mainWindow.AppWindow.MoveInZOrderAtTop();
                    }

                    return;
                }

            default:
                {
                    Window window = currentXamlWindowReference.Window;

                    // While window is closing, currentXamlWindowReference can still retrieve the window,
                    // just ignore it
                    if (window.AppWindow is not null)
                    {
                        window.SwitchTo();
                        window.AppWindow.MoveInZOrderAtTop();
                    }

                    return;
                }
        }
    }
}
