//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Behaviors;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Launcher.Core;
using Launcher.Core.Logging;
using Launcher.Core.Setting;
using Launcher.Service.Navigation;
using Launcher.Service.Navigation.Message;
using Launcher.Service.Notification;
using Launcher.UI.Content;
using Launcher.UI.Xaml.Control;
using Launcher.UI.Xaml.View.Page;
using WinRT;

namespace Launcher.UI.Xaml.Behavior;

internal sealed class ServiceRecipientNavigationViewBehavior : BehaviorBase<NavigationView>,
    IRecipient<NavigationNavigateMessage>,
    IRecipient<NavigationGoBackMessage>,
    IRecipient<NavigationPaneToggleMessage>
{
    private const int NavigationCooldownMs = 800;

    private static long lastNavigationTimestamp;
    private IMessenger? messenger;

    public void Receive(NavigationNavigateMessage message)
    {
        if (AssociatedObject is not { } navigationView)
        {
            message.Result = NavigationResult.Failed;
            return;
        }

        message.Result = Navigate(navigationView, message.PageType, message.Data, message.SyncNavigationViewItem);
    }

    public void Receive(NavigationGoBackMessage message)
    {
        if (AssociatedObject is not { } navigationView || navigationView.Content.As<Frame>() is not { } frame)
        {
            message.Result = false;
            return;
        }

        if (frame.CanGoBack)
        {
            frame.GoBack();
            message.Result = SyncSelectedNavigationViewItem(navigationView, frame.Content?.GetType());
        }
    }

    public void Receive(NavigationPaneToggleMessage message)
    {
        if (AssociatedObject is not { } navigationView)
        {
            return;
        }

        navigationView.IsPaneOpen = !navigationView.IsPaneOpen;
    }

    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.IsPaneOpen = LocalSetting.Get(SettingKeys.IsNavPaneOpen, true);

        if (AssociatedObject.XamlRoot.XamlContext()?.ServiceProvider is { } serviceProvider)
        {
            messenger = serviceProvider.GetRequiredService<IMessenger>();
            messenger.Register<NavigationNavigateMessage>(this);
            messenger.Register<NavigationGoBackMessage>(this);
            messenger.Register<NavigationPaneToggleMessage>(this);
        }

        AssociatedObject.ItemInvoked += OnItemInvoked;
        AssociatedObject.PaneOpened += OnPaneStateChanged;
        AssociatedObject.PaneClosed += OnPaneStateChanged;

        Navigate(AssociatedObject, typeof(LauncherHomePage), NavigationExtraData.Default, true);
    }

    protected override bool Uninitialize()
    {
        messenger?.UnregisterAll(this);
        AssociatedObject.PaneOpened -= OnPaneStateChanged;
        AssociatedObject.PaneClosed -= OnPaneStateChanged;
        return base.Uninitialize();
    }

    private static IEnumerable<NavigationViewItem> EnumerateMenuItems(IList<object> items)
    {
        foreach (NavigationViewItem item in items.OfType<NavigationViewItem>())
        {
            yield return item;

            if (item.MenuItems.Count > 0)
            {
                foreach (NavigationViewItem subItem in EnumerateMenuItems(item.MenuItems))
                {
                    yield return subItem;
                }
            }
        }
    }

    private static void OnPaneStateChanged(NavigationView sender, object args)
    {
        ArgumentNullException.ThrowIfNull(sender);
        LocalSetting.Set(SettingKeys.IsNavPaneOpen, sender.IsPaneOpen);
    }

    private static void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer is not NavigationViewItem item)
        {
            return;
        }

        Type? targetType = args.IsSettingsInvoked
            ? typeof(SettingPage)
            : NavigationViewItemHelper.GetNavigateTo(item);

        // Ignore item that doesn't have nav type specified
        if (targetType is not null)
        {
            INavigationCompletionSource data = new NavigationExtraData(
                new EntranceNavigationTransitionInfo(),
                NavigationViewItemHelper.GetExtraData(item));
            Navigate(sender, targetType, data, syncNavigationViewItem: false);
        }
    }

    private static bool IsNavigationThrottled()
    {
        long now = Environment.TickCount64;
        long last = Interlocked.Read(ref lastNavigationTimestamp);
        if (now - last < NavigationCooldownMs)
        {
            return true;
        }

        Interlocked.Exchange(ref lastNavigationTimestamp, now);
        return false;
    }

    private static NavigationResult Navigate(NavigationView navigationView, Type pageType, INavigationCompletionSource data, bool syncNavigationViewItem = false)
    {
        if (navigationView.Content.As<Frame>() is not { } frame)
        {
            return NavigationResult.Failed;
        }

        Page? currentPage = frame.Content?.As<Page>();
        Type? currentPageType = currentPage?.GetType();

        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateNavigation(
            currentPageType is null ? "Empty" : TypeNameHelper.GetTypeDisplayName(currentPageType, fullName: false),
            TypeNameHelper.GetTypeDisplayName(pageType, fullName: false),
            "Navigation"));

        if (currentPageType == pageType)
        {
            System.Threading.CancellationToken token = currentPage is ScopedPage scopedPage ? scopedPage.CancellationToken : System.Threading.CancellationToken.None;
            NavigationExtraDataSupport.NotifyRecipientAsync(frame.Content, data, token).SafeForget();
            return NavigationResult.AlreadyNavigatedTo;
        }

        // Throttle: prevent frequent page switching
        if (IsNavigationThrottled())
        {
            // Revert NavigationView selection back to current page
            SyncSelectedNavigationViewItem(navigationView, currentPageType);

            if (navigationView.XamlRoot.XamlContext()?.ServiceProvider is { } sp)
            {
                IMessenger msg = sp.GetRequiredService<IMessenger>();
                msg.Send(InfoBarMessage.Warning(SH.ViewNavigationThrottleTitle, SH.ViewNavigationThrottleMessage));
            }

            return NavigationResult.Failed;
        }

        _ = syncNavigationViewItem && SyncSelectedNavigationViewItem(navigationView, pageType);

        bool navigated = false;
        try
        {
            navigated = data is ISupportNavigationTransitionInfo { TransitionInfo: { } transitionInfo }
                ? frame.Navigate(pageType, data, transitionInfo)
                : frame.Navigate(pageType, data);
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }

        if (navigated)
        {
            UpdatePaneScrollBarVisibility(navigationView, pageType);
        }

        return navigated ? NavigationResult.Succeed : NavigationResult.Failed;
    }

    private static void UpdatePaneScrollBarVisibility(NavigationView navigationView, Type pageType)
    {
        if (navigationView.FindDescendant<ScrollViewer>(sv => sv.Name == "MenuItemsScrollViewer") is { } scrollViewer)
        {
            scrollViewer.VerticalScrollBarVisibility = pageType == typeof(LauncherHomePage)
                ? ScrollBarVisibility.Hidden
                : ScrollBarVisibility.Auto;
        }
    }

    private static bool SyncSelectedNavigationViewItem(NavigationView navigationView, Type? pageType)
    {
        if (pageType is null)
        {
            return false;
        }

        if (pageType == typeof(SettingPage))
        {
            navigationView.SelectedItem = navigationView.SettingsItem;
            return true;
        }

        NavigationViewItem? target = EnumerateMenuItems(navigationView.MenuItems)
            .SingleOrDefault(menuItem => NavigationViewItemHelper.GetNavigateTo(menuItem) == pageType);

        if (target is null)
        {
            return false;
        }

        navigationView.SelectedItem = target;
        return true;
    }
}
