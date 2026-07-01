// Launcher - Submit Feedback Dialog

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Launcher.Core;
using Launcher.Core.IO;
using Launcher.Factory.ContentDialog;
using Launcher.Factory.Picker;
using Launcher.Service.Notification;
using Launcher.Service.RemoteConfig;
using System.IO;

namespace Launcher.UI.Xaml.View.Dialog;

internal sealed partial class SubmitFeedbackDialog : ContentDialog
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IMessenger messenger;
    private string? selectedImagePath;

    public SubmitFeedbackDialog(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        contentDialogFactory = serviceProvider.GetRequiredService<IContentDialogFactory>();
        fileSystemPickerInteraction = serviceProvider.GetRequiredService<IFileSystemPickerInteraction>();
        messenger = serviceProvider.GetRequiredService<IMessenger>();
    }

    public async ValueTask ShowFeedbackAsync()
    {
        await contentDialogFactory.EnqueueAndShowAsync(this).ShowTask.ConfigureAwait(false);
    }

    private void PickImage_Click(object sender, RoutedEventArgs e)
    {
        ValueResult<bool, ValueFile> result = fileSystemPickerInteraction.PickFile(
            SH.ViewFeedbackPickImageTitle, null, SH.ViewFeedbackPickImageFilter, "*.png;*.jpg;*.jpeg;*.gif;*.webp;*.bmp");
        if (result.IsOk)
        {
            selectedImagePath = result.Value;
            ImageNameText.Text = Path.GetFileName(selectedImagePath) ?? selectedImagePath;
            ClearImageBtn.IsEnabled = true;
        }
    }

    private void ClearImage_Click(object sender, RoutedEventArgs e)
    {
        selectedImagePath = null;
        ImageNameText.Text = SH.ViewFeedbackNoImageSelected;
        ClearImageBtn.IsEnabled = false;
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ContentDialogButtonClickDeferral deferral = args.GetDeferral();
        args.Cancel = true;

        string content = ContentBox.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(content))
        {
            StatusText.Text = SH.ViewFeedbackContentRequired;
            StatusText.Visibility = Visibility.Visible;
            deferral.Complete();
            return;
        }

        StatusText.Visibility = Visibility.Collapsed;
        UploadProgress.Visibility = Visibility.Visible;
        IsPrimaryButtonEnabled = false;

        List<string> imageUrls = [];
        if (selectedImagePath is not null)
        {
            string? url = await FeedbackService.UploadImageAsync(selectedImagePath).ConfigureAwait(true);
            if (url is not null)
            {
                imageUrls.Add(url);
            }
        }

        bool ok = await FeedbackService.SubmitFeedbackAsync(
            content,
            ContactBox.Text?.Trim() ?? "",
            LauncherRuntime.Version.ToString(),
            imageUrls).ConfigureAwait(true);

        if (ok)
        {
            messenger.Send(InfoBarMessage.Success(SH.ViewFeedbackSubmitSuccess));
            args.Cancel = false;
        }
        else
        {
            StatusText.Text = SH.ViewFeedbackSubmitFailed;
            StatusText.Visibility = Visibility.Visible;
            UploadProgress.Visibility = Visibility.Collapsed;
            IsPrimaryButtonEnabled = true;
        }

        deferral.Complete();
    }
}
