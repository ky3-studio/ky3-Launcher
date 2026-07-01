//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Launcher.Core.DataTransfer;
using Launcher.Factory.ContentDialog;
using Launcher.UI.Xaml.Control;
using Launcher.ViewModel.Wiki;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace Launcher.UI.Xaml.View.Page;

internal sealed partial class EmotionIconPage : ScopedPage
{
    public EmotionIconPage()
    {
        InitializeComponent();
    }

    protected override void LoadingOverride()
    {
        InitializeDataContext<EmotionIconViewModel>();
    }

    private async void OnEmotionIconDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (e.OriginalSource is FrameworkElement { DataContext: EmotionIconEntry entry })
        {
            ContentDialog dialog = new()
            {
                XamlRoot = XamlRoot,
                Content = new Image
                {
                    Source = new BitmapImage(entry.ImageUri),
                    Width = 200,
                    Height = 200,
                    Stretch = Stretch.Uniform,
                },
                PrimaryButtonText = SH.ViewEmotionIconCopyButton,
                CloseButtonText = SH.ViewEmotionIconCloseButton,
                DefaultButton = ContentDialogButton.Primary,
            };

            dialog.PrimaryButtonClick += async (s, args) =>
            {
                args.Cancel = true;
                try
                {
                    IHttpClientFactory factory = Ioc.Default.GetRequiredService<IHttpClientFactory>();
                    IClipboardProvider clipboard = Ioc.Default.GetRequiredService<IClipboardProvider>();
                    using HttpClient client = factory.CreateClient();
                    byte[] data = await client.GetByteArrayAsync(entry.ImageUri);
                    using InMemoryRandomAccessStream stream = new();
                    await stream.WriteAsync(data.AsBuffer());
                    stream.Seek(0);
                    await clipboard.SetBitmapAsync(stream);
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }
                dialog.Hide();
            };

            await Ioc.Default.GetRequiredService<IContentDialogFactory>()
                .EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);
        }
    }
}
