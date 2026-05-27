//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using kyxsan.UI.Xaml.Control;
using kyxsan.ViewModel.Home;
using kyxsan.Service.RemoteConfig;
using kyxsan.UI.Content;
using kyxsan.Factory.ContentDialog;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace kyxsan.UI.Xaml.View.Page;

internal sealed partial class AnnouncementPage : ScopedPage
{
    public AnnouncementPage()
    {
        InitializeComponent();
        Loaded += OnAnnouncementPageLoaded;
        Unloaded += OnAnnouncementPageUnloaded;
    }

    private async void OnAnnouncementPageLoaded(object sender, RoutedEventArgs e)
    {
        RenderAnnouncements(AppAnnouncementService.Current);
        AppAnnouncementService.Changed += OnAppAnnouncementsChanged;
        AppAnnouncementService.StartPolling();
        AppAnnouncementService.MarkAllSeen();
        await AppAnnouncementService.ForceRefreshAsync().ConfigureAwait(false);
    }

    private void OnAnnouncementPageUnloaded(object sender, RoutedEventArgs e)
    {
        AppAnnouncementService.Changed -= OnAppAnnouncementsChanged;
    }

    private void OnAppAnnouncementsChanged(List<AppAnnouncementService.AppAnnouncement> items)
    {
        DispatcherQueue?.TryEnqueue(() => RenderAnnouncements(items));
    }

    private void RenderAnnouncements(List<AppAnnouncementService.AppAnnouncement> items)
    {
        AppAnnouncementPanel.Children.Clear();
        if (items.Count == 0)
        {
            AppAnnouncementCard.Visibility = Visibility.Collapsed;
            return;
        }

        foreach (AppAnnouncementService.AppAnnouncement ann in items)
        {
            SolidColorBrush accentBrush = ann.Type switch
            {
                "warning" => new SolidColorBrush(Colors.Orange),
                "error" => new SolidColorBrush(Colors.Red),
                "success" => new SolidColorBrush(Colors.Green),
                _ => new SolidColorBrush(Colors.DodgerBlue),
            };

            // Acrylic card wrapper — unified with AcrylicGridCardStyle
            Grid cardWrapper = new()
            {
                Style = (Style)Application.Current.Resources["AcrylicGridCardStyle"],
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(0),
            };
            cardWrapper.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4) });
            cardWrapper.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Left accent bar
            Border accent = new()
            {
                Background = accentBrush,
                CornerRadius = new CornerRadius(8, 0, 0, 8),
                Width = 4,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            Grid.SetColumn(accent, 0);
            cardWrapper.Children.Add(accent);

            // Right content area
            StackPanel content = new() { Spacing = 8, Padding = new Thickness(16, 14, 16, 14) };
            Grid.SetColumn(content, 1);

            // Title
            TextBlock title = new()
            {
                Text = ann.Title,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Foreground = accentBrush,
            };
            content.Children.Add(title);

            // Content text
            if (!string.IsNullOrWhiteSpace(ann.Content))
            {
                TextBlock body = new()
                {
                    Text = ann.Content,
                    FontSize = 13,
                    TextWrapping = TextWrapping.Wrap,
                    Opacity = 0.85,
                };
                content.Children.Add(body);
            }

            if (!string.IsNullOrWhiteSpace(ann.ImageUrl) && Uri.TryCreate(ann.ImageUrl, UriKind.Absolute, out Uri? imageUri))
            {
                Image img = new()
                {
                    MaxHeight = 200,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Stretch = Stretch.Uniform,
                };
                _ = LoadImageAsync(img, imageUri);
                Button imgBtn = new()
                {
                    Content = img,
                    Padding = new Thickness(0),
                    Background = new SolidColorBrush(Colors.Transparent),
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                };
                imgBtn.Click += async (s, args) =>
                {
                    FrameworkElement fe = (FrameworkElement)s;
                    double screenW = fe.XamlRoot.Size.Width;
                    double screenH = fe.XamlRoot.Size.Height;
                    Image fullImg = new()
                    {
                        Stretch = Stretch.Uniform,
                        Source = img.Source,
                        MaxWidth = screenW * 0.9,
                        MaxHeight = screenH * 0.8,
                    };
                    ContentDialog dialog = new()
                    {
                        XamlRoot = fe.XamlRoot,
                        Content = fullImg,
                        CloseButtonText = "关闭",
                        DefaultButton = ContentDialogButton.Close,
                    };
                    dialog.Resources["ContentDialogMaxWidth"] = screenW * 0.9 + 48;
                    IContentDialogFactory contentDialogFactory = fe.XamlRoot.XamlContext()!.ServiceProvider.GetRequiredService<IContentDialogFactory>();
                    await contentDialogFactory.EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);
                };
                content.Children.Add(imgBtn);
            }

            // Link button
            if (!string.IsNullOrWhiteSpace(ann.LinkUrl) && Uri.TryCreate(ann.LinkUrl, UriKind.Absolute, out Uri? linkUri))
            {
                HyperlinkButton link = new()
                {
                    Content = string.IsNullOrWhiteSpace(ann.LinkText) ? ann.LinkUrl : ann.LinkText,
                    NavigateUri = linkUri,
                    Padding = new Thickness(0),
                    FontSize = 13,
                };
                content.Children.Add(link);
            }

            cardWrapper.Children.Add(content);
            AppAnnouncementPanel.Children.Add(cardWrapper);
        }

        AppAnnouncementCard.Visibility = Visibility.Visible;
    }

    private static async Task LoadImageAsync(Image img, Uri uri)
    {
        try
        {
            byte[] data = await AppAnnouncementService.Http.GetByteArrayAsync(uri).ConfigureAwait(false);
            img.DispatcherQueue?.TryEnqueue(async () =>
            {
                try
                {
                    InMemoryRandomAccessStream stream = new();
                    await stream.WriteAsync(data.AsBuffer());
                    stream.Seek(0);
                    BitmapImage bmp = new();
                    await bmp.SetSourceAsync(stream);
                    img.Source = bmp;
                }
                catch { }
            });
        }
        catch { }
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<AnnouncementViewModel>();
    }
}
