using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using kyxsan.Service.Notification;
using CommunityToolkit.Mvvm.Input;

namespace kyxsan.Service.RemoteConfig;

internal sealed class AppAnnouncementNotifier
{
    private readonly IMessenger messenger;
    private readonly List<AppAnnouncementService.AppAnnouncement> pendingAnnouncements = [];
    private bool windowIsActive = true;

    public AppAnnouncementNotifier(IMessenger messenger)
    {
        this.messenger = messenger;
    }

    public void Attach(Window mainWindow)
    {
        mainWindow.Activated += (_, e) =>
        {
            bool isActive = e.WindowActivationState != WindowActivationState.Deactivated;
            if (isActive && !windowIsActive && pendingAnnouncements.Count > 0)
            {
                FlushPending();
            }
            windowIsActive = isActive;
        };
    }

    public void Start()
    {
        AppAnnouncementService.Changed += OnAnnouncementsChanged;
        AppAnnouncementService.StartPolling();
        OnAnnouncementsChanged(AppAnnouncementService.Current);
    }

    private void OnAnnouncementsChanged(List<AppAnnouncementService.AppAnnouncement> items)
    {
        List<AppAnnouncementService.AppAnnouncement> newItems = AppAnnouncementService.FilterNew(items);
        if (newItems.Count == 0)
        {
            return;
        }

        if (windowIsActive)
        {
            foreach (AppAnnouncementService.AppAnnouncement ann in newItems)
            {
                messenger.Send(ToInfoBar(ann));
            }
            AppAnnouncementService.MarkAllSeen();
        }
        else
        {
            pendingAnnouncements.AddRange(newItems);
        }
    }

    private void FlushPending()
    {
        foreach (AppAnnouncementService.AppAnnouncement ann in pendingAnnouncements)
        {
            messenger.Send(ToInfoBar(ann));
        }
        pendingAnnouncements.Clear();
        AppAnnouncementService.MarkAllSeen();
    }

    private static InfoBarMessage ToInfoBar(AppAnnouncementService.AppAnnouncement ann)
    {
        InfoBarSeverity severity = ann.Type switch
        {
            "error" => InfoBarSeverity.Error,
            "warning" => InfoBarSeverity.Warning,
            "success" => InfoBarSeverity.Success,
            _ => InfoBarSeverity.Informational,
        };

        string? actionText = null;
        ICommand? actionCommand = null;
        if (!string.IsNullOrEmpty(ann.LinkUrl))
        {
            actionText = string.IsNullOrEmpty(ann.LinkText) ? ann.LinkUrl : ann.LinkText;
            string url = ann.LinkUrl;
            actionCommand = new RelayCommand(() =>
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            });
        }

        return new InfoBarMessage
        {
            Severity = severity,
            Title = ann.Title,
            Message = ann.Content,
            ImageUrl = string.IsNullOrEmpty(ann.ImageUrl) ? null : ann.ImageUrl,
            ActionButtonContent = actionText,
            ActionButtonCommand = actionCommand,
            MilliSecondsDelay = 0,
        };
    }
}
