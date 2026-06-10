// kyxsan - Submit Feedback Dialog

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core;
using kyxsan.Core.IO;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Service.Notification;
using kyxsan.Service.RemoteConfig;
using System.IO;

namespace kyxsan.UI.Xaml.View.Dialog;

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
            "选择反馈图片", null, "图片文件", "*.png;*.jpg;*.jpeg;*.gif;*.webp;*.bmp");
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
        ImageNameText.Text = "未选择图片";
        ClearImageBtn.IsEnabled = false;
    }

    private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        ContentDialogButtonClickDeferral deferral = args.GetDeferral();
        args.Cancel = true;

        string content = ContentBox.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(content))
        {
            StatusText.Text = "请填写反馈内容";
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
            kyxsanRuntime.Version.ToString(),
            imageUrls).ConfigureAwait(true);

        if (ok)
        {
            messenger.Send(InfoBarMessage.Success("感谢您的反馈，我们会认真阅读！"));
            args.Cancel = false;
        }
        else
        {
            StatusText.Text = "提交失败，请检查网络连接后重试";
            StatusText.Visibility = Visibility.Visible;
            UploadProgress.Visibility = Visibility.Collapsed;
            IsPrimaryButtonEnabled = true;
        }

        deferral.Complete();
    }
}
