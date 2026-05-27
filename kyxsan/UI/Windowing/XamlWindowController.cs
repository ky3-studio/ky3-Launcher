//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using ABI.Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core;
using kyxsan.Core.LifeCycle;
using kyxsan.Core.Logging;
using kyxsan.Core.Property;
using kyxsan.Core.Setting;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service;
using kyxsan.UI.Content;
using kyxsan.UI.Windowing.Abstraction;
using kyxsan.UI.Xaml.Control.Theme;
using kyxsan.UI.Xaml.Media.Backdrop;
using kyxsan.UI.Xaml.View.Window;
using kyxsan.Win32.UI.Shell;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using Windows.Graphics;
using Windows.UI;
using WinRT;
using WinRT.Interop;
using AppWindowTitleBar = Microsoft.UI.Windowing.AppWindowTitleBar;

namespace kyxsan.UI.Windowing;

[SuppressMessage("", "CA1001")]
[SuppressMessage("", "SA1204")]
internal sealed class XamlWindowController
{
    private readonly Type windowType;
    private readonly Window window;
    private readonly bool hasCustomSystemBackdrop;

    private readonly XamlWindowSubclass subclass;
    private readonly XamlWindowNonRude nonRude;

    public XamlWindowController(Window window, IServiceProvider serviceProvider)
    {
        windowType = window.GetType();
        this.window = window;
        Debug.Assert(serviceProvider is IServiceScope);
        ServiceProvider = serviceProvider;

        // Subclassing and NonRudeHWND are standard infrastructure.
        subclass = new(window);
        nonRude = new(window.GetWindowHandle());

        window.AppWindow.Title = SH.FormatAppNameAndVersion(kyxsanRuntime.Version);
        window.AppWindow.SetIcon(InstalledLocation.GetAbsolutePath("Assets/Logo.ico"));

        // ExtendContentIntoTitleBar
        if (window is IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;
            appTitleBar.IconShowOptions = IconShowOptions.HideIconAndSystemMenu;
            appTitleBar.ExtendsContentIntoTitleBar = true;

            UpdateTitleButtonColor(default!, default!);
            xamlWindow.TitleBarCaptionAccess.ActualThemeChanged += UpdateTitleButtonColor;

            if (xamlWindow.TitleBarCaptionAccess is not TitleBar)
            {
                XamlWindowRegionRects.Update(window);
                xamlWindow.TitleBarCaptionAccess.SizeChanged += OnWindowSizeChanged;
            }
        }

        // Size stuff
        AppOptions appOptions = serviceProvider.GetRequiredService<AppOptions>();
        bool restoredPlacement = false;
        if (window is MainWindow && appOptions.RememberWindowSize.Value)
        {
            int savedWidth = LocalSetting.Get(SettingKeys.LastWindowWidth, 0);
            int savedHeight = LocalSetting.Get(SettingKeys.LastWindowHeight, 0);
            int savedX = LocalSetting.Get(SettingKeys.LastWindowX, int.MinValue);
            int savedY = LocalSetting.Get(SettingKeys.LastWindowY, int.MinValue);

            if (savedWidth > 0 && savedHeight > 0)
            {
                window.AppWindow.Resize(new SizeInt32(savedWidth, savedHeight));

                if (savedX != int.MinValue && savedY != int.MinValue)
                {
                    window.AppWindow.Move(new PointInt32(savedX, savedY));
                    restoredPlacement = true;
                }
            }
        }

        if (!restoredPlacement)
        {
            if (window is IXamlWindowHasInitSize xamlWindow2)
            {
                window.AppWindow.Resize(xamlWindow2.InitSize);
            }

            // Center the window on screen
            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(window.AppWindow.Id, DisplayAreaFallback.Primary);
            if (displayArea is not null)
            {
                RectInt32 workArea = displayArea.WorkArea;
                SizeInt32 windowSize = window.AppWindow.Size;
                window.AppWindow.Move(new PointInt32(
                    workArea.X + ((workArea.Width - windowSize.Width) / 2),
                    workArea.Y + ((workArea.Height - windowSize.Height) / 2)));
            }
        }

        // window.AppWindow.EnablePlacementPersistence(guid, window is MainWindow, default, PlacementPersistenceBehaviorFlags.Default, windowName);
        // EnablePlacementRestoration(window); // Disabled to use InitSize every time

        window.Content.As<FrameworkElement>().Loading += OnWindowContentLoading;

        window.AppWindow.Show(true);
        window.AppWindow.MoveInZOrderAtTop();

        hasCustomSystemBackdrop = window.SystemBackdrop is not null;

        // SystemBackdrop
        UpdateSystemBackdrop(appOptions.BackdropType.Value);
        BackdropTypeCallback = appOptions.BackdropType.WithValueChangedCallback(static (value, controller) => controller.UpdateSystemBackdrop(value), this);

        // Sync XAML RequestedTheme with saved ElementTheme
        ElementTheme savedTheme = appOptions.ElementTheme.Value;
        if (savedTheme is not ElementTheme.Default && window.Content is FrameworkElement contentElement)
        {
            contentElement.RequestedTheme = savedTheme;
        }

        subclass.Initialize();
        window.Closed += OnWindowClosed;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal IServiceProvider ServiceProvider { get; }

    private IObservableProperty<BackdropType>? BackdropTypeCallback { get; }

    public bool TrySetTaskbarProgress(TBPFLAG state, ulong value, ulong maximum)
    {
        try
        {
            subclass.SetTaskbarProgress(state, value, maximum);
            return true;
        }
        catch (Exception ex)
        {
            Debugger.Break();
            SentrySdk.CaptureException(ex);
            return false;
        }
    }

    private static void EnablePlacementRestoration(Window window)
    {
        IObjectReference objRefAppWindowExperimental = Unsafe.As<IWinRTObject>(window.AppWindow).NativeObject.As<IUnknownVftbl>(IAppWindowExperimentalMethods.IID);
        IAppWindowExperimentalMethods.set_PlacementRestorationBehavior(objRefAppWindowExperimental, PlacementRestorationBehavior.All);

        string windowName = TypeNameHelper.GetTypeDisplayName(window);
        byte[] data = CryptographicOperations.HashData(HashAlgorithmName.MD5, Encoding.UTF8.GetBytes(windowName));
        Guid guid = MemoryMarshal.AsRef<Guid>(data);
        IAppWindowExperimentalMethods.set_PersistedStateId(objRefAppWindowExperimental, guid);
    }

    private void OnWindowContentLoading(FrameworkElement element, object e)
    {
        element.Loading -= OnWindowContentLoading;
        element.XamlRoot.ContentIsland.AppData = new XamlContext
        {
            ServiceProvider = ServiceProvider,
        };
    }

    private void OnWindowClosed(object sender, WindowEventArgs args)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateDebug("WindowClosing", "XamlWindowController", [("type", TypeNameHelper.GetTypeDisplayName(window, false))]));

        if (args.Handled)
        {
            return;
        }

        if (window is IXamlWindowClosedHandler handler)
        {
            handler.OnWindowClosing(out bool cancel);
            if (cancel)
            {
                args.Handled = true;
                return;
            }
        }

        if (!XamlApplicationLifetime.Exiting)
        {
            IServiceProviderIsKeyedService isKeyedService = ServiceProvider.GetRequiredService<IServiceProviderIsKeyedService>();
            ICurrentXamlWindowReference currentXamlWindowReference = isKeyedService.IsKeyedService(typeof(ICurrentXamlWindowReference), windowType)
                ? ServiceProvider.GetRequiredKeyedService<ICurrentXamlWindowReference>(windowType)
                : ServiceProvider.GetRequiredService<ICurrentXamlWindowReference>();

            if (currentXamlWindowReference.Window == window)
            {
                // Only a CurrentWindow can show dialogs
                // Some users might try to close the window while a dialog is showing
                if (ServiceProvider.GetRequiredService<IContentDialogFactory>().IsDialogShowing)
                {
                    args.Handled = true;
                    return;
                }

                currentXamlWindowReference.Window = default!;
            }
        }

        // Detach events
        window.Closed -= OnWindowClosed;

        // Save window placement for MainWindow
        if (window is MainWindow)
        {
            try
            {
                AppOptions options = ServiceProvider.GetRequiredService<AppOptions>();
                if (options.RememberWindowSize.Value)
                {
                    SizeInt32 size = window.AppWindow.Size;
                    PointInt32 position = window.AppWindow.Position;
                    LocalSetting.Set(SettingKeys.LastWindowWidth, size.Width);
                    LocalSetting.Set(SettingKeys.LastWindowHeight, size.Height);
                    LocalSetting.Set(SettingKeys.LastWindowX, position.X);
                    LocalSetting.Set(SettingKeys.LastWindowY, position.Y);
                }
            }
            catch
            {
            }
        }

        if (window is IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            xamlWindow.TitleBarCaptionAccess.ActualThemeChanged -= UpdateTitleButtonColor;
            xamlWindow.TitleBarCaptionAccess.SizeChanged -= OnWindowSizeChanged;
        }

        // Dispose components
        subclass.Dispose();
        nonRude.Dispose();

        (window as IXamlWindowClosedHandler)?.OnWindowClosed();

        // Dispose the service scope
        Unsafe.As<IServiceScope>(ServiceProvider).Dispose();
        window.UninitializeController();
    }

    private bool UpdateSystemBackdrop(BackdropType backdropType)
    {
        if (hasCustomSystemBackdrop)
        {
            return true;
        }

        try
        {
            window.SystemBackdrop = backdropType switch
            {
                BackdropType.Transparent => new TransparentBackdrop(),
                BackdropType.MicaAlt => new SafeMicaBackdrop(MicaKind.BaseAlt),
                BackdropType.Mica => new SafeMicaBackdrop(MicaKind.Base),
                BackdropType.Acrylic => new SafeDesktopAcrylicBackdrop(),
                BackdropType.AcrylicThin => new SafeDesktopAcrylicBackdrop(DesktopAcrylicKind.Thin),
                _ => null,
            };
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }

    private void UpdateTitleButtonColor(FrameworkElement discardElement, object e)
    {
        if (window is not IXamlWindowExtendContentIntoTitleBar xamlWindow)
        {
            return;
        }

        AppWindowTitleBar appTitleBar = window.AppWindow.TitleBar;

        appTitleBar.ButtonBackgroundColor = Colors.Transparent;
        appTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

        bool isDarkMode = ThemeHelper.IsDarkMode(xamlWindow.TitleBarCaptionAccess.ActualTheme);

        Color systemBaseLowColor = SystemColors.BaseLowColor(isDarkMode);
        appTitleBar.ButtonHoverBackgroundColor = systemBaseLowColor;

        Color systemBaseMediumLowColor = SystemColors.BaseMediumLowColor(isDarkMode);
        appTitleBar.ButtonPressedBackgroundColor = systemBaseMediumLowColor;

        // The Foreground doesn't accept Alpha channel. So we translate it to gray.
        byte light = (byte)((systemBaseMediumLowColor.R + systemBaseMediumLowColor.G + systemBaseMediumLowColor.B) / 3);
        byte result = (byte)(systemBaseMediumLowColor.A / 255.0 * light);
        appTitleBar.ButtonInactiveForegroundColor = Color.FromArgb(0xFF, result, result, result);

        Color systemBaseHighColor = SystemColors.BaseHighColor(isDarkMode);
        appTitleBar.ButtonForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonHoverForegroundColor = systemBaseHighColor;
        appTitleBar.ButtonPressedForegroundColor = systemBaseHighColor;
    }

    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        XamlWindowRegionRects.Update(window);
    }
}