//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using kyxsan.Core;
using kyxsan.Core.DataTransfer;
using kyxsan.Core.IO.Http.Loopback;
using kyxsan.Core.IO.Http.Proxy;
using kyxsan.Core.Logging;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service;
using kyxsan.Service.Notification;
using kyxsan.Web.kyxsan;
using kyxsan.Web.kyxsan.Algolia;
using kyxsan.Web.Response;
using System.Runtime.InteropServices;
using Windows.System;

namespace kyxsan.ViewModel.Feedback;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class FeedbackViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IClipboardProvider clipboardProvider;
    private readonly IServiceProvider serviceProvider;
    private readonly CultureOptions cultureOptions;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial FeedbackViewModel(IServiceProvider serviceProvider);

    public static HttpProxyUsingSystemProxy DynamicHttpProxy { get => HttpProxyUsingSystemProxy.Instance; }

    public partial RuntimeOptions RuntimeOptions { get; }

    public partial LoopbackSupport LoopbackSupport { get; }

    [ObservableProperty]
    public partial string? SearchText { get; set; }

    [ObservableProperty]
    public partial List<AlgoliaHit>? SearchResults { get; set; }

    [ObservableProperty]
    public partial IPInformation? IPInformation { get; private set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        IPInformation? info;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            kyxsanInfrastructureClient kyxsanInfrastructureClient = scope.ServiceProvider.GetRequiredService<kyxsanInfrastructureClient>();
            Response<IPInformation> resp = await kyxsanInfrastructureClient.GetIPInformationAsync(token).ConfigureAwait(false);
            ResponseValidator.TryValidate(resp, messenger, out info);
        }

        info ??= IPInformation.Default;
        await taskContext.SwitchToMainThreadAsync();
        IPInformation = info;

        return true;
    }

    [Command("NavigateToUriCommand")]
    private static async Task NavigateToUri(string? uri)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Navigate to uri", "FeedbackViewModel.Command", [("uri", uri ?? "<null>")]));

        if (string.IsNullOrEmpty(uri))
        {
            return;
        }

        await Launcher.LaunchUriAsync(uri.ToUri());
    }

    [Command("SearchDocumentCommand")]
    private async Task SearchDocumentAsync(string? search)
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Search", "FeedbackViewModel.Command", [("text", search ?? "<null>")]));

        IsInitialized = false;
        SearchResults = null;

        if (string.IsNullOrEmpty(search))
        {
            IsInitialized = true;
            return;
        }

        string language = LocaleNames.GetLanguageCodeForDocumentationSearchFromLocaleName(cultureOptions.LocaleName);
        AlgoliaResponse? response;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            kyxsanDocumentationClient kyxsanDocumentationClient = scope.ServiceProvider.GetRequiredService<kyxsanDocumentationClient>();
            response = await kyxsanDocumentationClient.QueryAsync(search, language).ConfigureAwait(false);
        }

        await taskContext.SwitchToMainThreadAsync();
        if (response is { Results: [{ Hits: { Count: > 0 } hits }, ..] })
        {
            SearchResults = [.. hits.DistinctBy(hit => hit.Url)];
        }
        else
        {
            SearchResults = null;
        }

        IsInitialized = true;
    }

    [Command("CopyDeviceIdCommand")]
    private async Task CopyDeviceIdAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Copy device id", "FeedbackViewModel.Command"));

        try
        {
            await clipboardProvider.SetTextAsync(kyxsanRuntime.DeviceId).ConfigureAwait(false);
            messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingCopyDeviceIdSuccess));
        }
        catch (COMException ex)
        {
            messenger.Send(InfoBarMessage.Error(ex));
        }
    }

    [Command("EnableLoopbackCommand")]
    private async Task EnableLoopbackAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Enable Loopback", "FeedbackViewModel.Command"));

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewDialogFeedbackEnableLoopbackTitle, SH.ViewDialogFeedbackEnableLoopbackContent)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            await taskContext.SwitchToMainThreadAsync();
            LoopbackSupport.EnableLoopback();
        }
    }
}