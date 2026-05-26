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
using kyxsan.Core.Database;
using kyxsan.Core.DataTransfer;
using kyxsan.Core.ExceptionService;
using kyxsan.Core.IO;
using kyxsan.Core.Logging;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Factory.Progress;
using kyxsan.Model.Entity;
using kyxsan.Service.GachaLog;
using kyxsan.Service.GachaLog.QueryProvider;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.Data;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.Win32.Foundation;
using System.IO;
using System.Runtime.InteropServices;

namespace kyxsan.ViewModel.GachaLog;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class GachaLogViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IServiceProvider serviceProvider;
    private readonly IProgressFactory progressFactory;
    private readonly IGachaLogService gachaLogService;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

    private bool suppressCurrentItemChangedHandling;
    private GachaLogServiceMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial GachaLogViewModel(IServiceProvider serviceProvider);

    public IAdvancedDbCollectionView<GachaArchive>? Archives
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentArchiveChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentArchiveChanged);
        }
    }

    public GachaStatistics? Statistics
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
            {
                field?.HistoryWishes.MoveCurrentToFirst();
            }
        }
    }

    [ObservableProperty]
    public partial bool IsAggressiveRefresh { get; set; }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        try
        {
            if (!await metadataService.InitializeAsync().ConfigureAwait(false))
            {
                return false;
            }

            metadataContext = await metadataService.GetContextAsync<GachaLogServiceMetadataContext>(token).ConfigureAwait(false);
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                IAdvancedDbCollectionView<GachaArchive> archives = await gachaLogService.GetArchiveCollectionAsync().ConfigureAwait(false);
                await taskContext.SwitchToMainThreadAsync();
                Archives = archives;
                Archives.MoveCurrentTo(Archives.Source.SelectedOrFirstOrDefault());
            }

            if (Archives.CurrentItem is null)
            {
                return true;
            }
        }
        catch (OperationCanceledException)
        {
        }

        return false;
    }

    protected override void UninitializeOverride()
    {
        using (Archives?.SuppressChangeCurrentItem())
        {
            Archives = default;
        }
    }

    private void OnCurrentArchiveChanged(object? sender, object? e)
    {
        if (suppressCurrentItemChangedHandling)
        {
            return;
        }

        UpdateStatisticsAsync(Archives?.CurrentItem).SafeForget();
    }

    [Command("RefreshByWebCacheCommand")]
    private async Task RefreshByWebCacheAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh gachalog", "GachaLogViewModel.Command", [("source", "WebCache")]));

        await PrivateRefreshAsync(RefreshOptionKind.WebCache).ConfigureAwait(false);
    }

    [Command("RefreshBySTokenCommand")]
    private async Task RefreshBySTokenAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh gachalog", "GachaLogViewModel.Command", [("source", "SToken")]));

        await PrivateRefreshAsync(RefreshOptionKind.SToken).ConfigureAwait(false);
    }

    [Command("RefreshByManualInputCommand")]
    private async Task RefreshByManualInputAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Refresh gachalog", "GachaLogViewModel.Command", [("source", "Manual Input")]));

        await PrivateRefreshAsync(RefreshOptionKind.ManualInput).ConfigureAwait(false);
    }

    [Command("GetGachaUrlCommand")]
    private async Task GetGachaUrlAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory2.CreateUI("Get gacha url", "GachaLogViewModel.Command", [("source", "LogFile")]));

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGachaLogQueryProvider provider = scope.ServiceProvider.GetRequiredKeyedService<IGachaLogQueryProvider>(RefreshOptionKind.LogFile);
            (bool isOk, GachaLogQuery query) = await provider.GetQueryAsync().ConfigureAwait(false);

            if (!isOk)
            {
                if (!string.IsNullOrEmpty(query.Message))
                {
                    messenger.Send(InfoBarMessage.Warning(query.Message));
                }

                return;
            }

            string baseUrl = query.IsOversea
                ? "https://gs.hoyoverse.com/genshin/event/e20190909gacha-v3/index.html"
                : "https://webstatic.mihoyo.com/hk4e/event/e20190909gacha-v3/index.html";

            string queryString = query.Query;
            if (!queryString.StartsWith('?'))
            {
                queryString = "?" + queryString;
            }

            string fullUrl = baseUrl + queryString;
            string serverName = query.IsOversea ? SH.ServiceGachaLogUrlProviderLogFileServerOversea : SH.ServiceGachaLogUrlProviderLogFileServerCN;
            IClipboardProvider clipboard = scope.ServiceProvider.GetRequiredService<IClipboardProvider>();
            await clipboard.SetTextAsync(fullUrl).ConfigureAwait(false);
            messenger.Send(InfoBarMessage.Success(SH.FormatServiceGachaLogUrlProviderLogFileUrlCopied(serverName)));
        }
    }

    private async ValueTask PrivateRefreshAsync(RefreshOptionKind optionKind)
    {
        GachaLogQuery query;
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            IGachaLogQueryProvider provider = scope.ServiceProvider.GetRequiredKeyedService<IGachaLogQueryProvider>(optionKind);
            (bool isOk, query) = await provider.GetQueryAsync().ConfigureAwait(false);

            if (!isOk)
            {
                if (!string.IsNullOrEmpty(query.Message))
                {
                    messenger.Send(InfoBarMessage.Warning(query.Message));
                }

                return;
            }
        }

        RefreshStrategyKind strategy = IsAggressiveRefresh ? RefreshStrategyKind.AggressiveMerge : RefreshStrategyKind.LazyMerge;

        GachaLogRefreshProgressDialog dialog;
        try
        {
            dialog = await contentDialogFactory.CreateInstanceAsync<GachaLogRefreshProgressDialog>(serviceProvider).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        BlockDeferral hideToken;
        try
        {
            hideToken = await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false);
        }
        catch (COMException ex)
        {
            if (ex.HResult is HRESULT.E_ASYNC_OPERATION_NOT_STARTED)
            {
                messenger.Send(InfoBarMessage.Error(ex));
                return;
            }

            throw;
        }

        IProgress<GachaLogFetchStatus> progress = progressFactory.CreateForMainThread<GachaLogFetchStatus>(dialog.OnReport);
        bool authkeyValid;

        try
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                try
                {
                    try
                    {
                        suppressCurrentItemChangedHandling = true;
                        ArgumentNullException.ThrowIfNull(metadataContext);
                        authkeyValid = await gachaLogService.RefreshGachaLogAsync(metadataContext, query, strategy, progress, CancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        suppressCurrentItemChangedHandling = false;
                        await UpdateStatisticsAsync(Archives?.CurrentItem).ConfigureAwait(false);
                    }
                }
                catch (kyxsanException ex)
                {
                    authkeyValid = false;
                    messenger.Send(InfoBarMessage.Error(ex));
                }
            }
        }
        catch (OperationCanceledException)
        {
            authkeyValid = true;
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelGachaLogRefreshOperationCancel));
        }

        await taskContext.SwitchToMainThreadAsync();
        if (authkeyValid)
        {
            hideToken.Dispose();
        }
        else
        {
            dialog.Title = SH.ViewModelGachaLogRefreshFail;
            dialog.PrimaryButtonText = SH.ContentDialogConfirmPrimaryButtonText;
            dialog.DefaultButton = ContentDialogButton.Primary;
        }
    }

    [Command("RemoveArchiveCommand")]
    private async Task RemoveArchiveAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Remove archive", "GachaLogViewModel.Command"));

        if (Archives?.CurrentItem is null)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(
                SH.FormatViewModelGachaLogRemoveArchiveTitle(Archives.CurrentItem.Uid),
                SH.ViewModelGachaLogRemoveArchiveDescription)
            .ConfigureAwait(false);

        if (result is not ContentDialogResult.Primary)
        {
            return;
        }

        using (await EnterCriticalSectionAsync().ConfigureAwait(false))
        {
            await gachaLogService.RemoveArchiveAsync(Archives.CurrentItem).ConfigureAwait(false);
        }
    }

    [Command("ImportUIGFCommand")]
    private async Task ImportUIGFAsync()
    {
        (bool isOk, ValueFile file) = fileSystemPickerInteraction.PickFile(SH.ViewModelGachaLogExportFileType, null, "JSON", "*.json");
        if (!isOk || !file.HasValue)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        using FileStream stream = File.OpenRead(file);
        (int count, Guid archiveId) = await gachaLogService.ImportFromUIGFAsync(metadataContext, stream).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        messenger.Send(InfoBarMessage.Success(SH.FormatViewModelGachaLogUIGFImportSuccess(count)));

        if (count > 0 && archiveId != default)
        {
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                GachaArchive archive = await gachaLogService.EnsureArchiveInCollectionAsync(archiveId).ConfigureAwait(false);
                await taskContext.SwitchToMainThreadAsync();
                Archives?.MoveCurrentTo(archive);
                await UpdateStatisticsAsync(archive).ConfigureAwait(false);
            }
        }
    }

    [Command("ExportUIGFCommand")]
    private async Task ExportUIGFAsync()
    {
        if (Archives?.CurrentItem is not { } archive)
        {
            return;
        }

        ContentDialogResult formatResult = await contentDialogFactory
            .CreateForSelectionAsync(
                SH.ViewModelGachaLogExportFormatTitle,
                SH.ViewModelGachaLogExportFormatDescription,
                "UIGF v4.0",
                "UIGF v2.3")
            .ConfigureAwait(false);

        if (formatResult is ContentDialogResult.None)
        {
            return;
        }

        bool useLegacyFormat = formatResult is ContentDialogResult.Secondary;

        string prefix = useLegacyFormat ? "UIGF_v2.3" : "UIGF";
        (bool isOk, ValueFile file) = fileSystemPickerInteraction.SaveFile(SH.ViewModelGachaLogExportFileType, $"{prefix}_{archive.Uid}_{DateTime.Now:yyyyMMddHHmmss}.json", "JSON", "*.json");
        if (!isOk || !file.HasValue)
        {
            return;
        }

        await taskContext.SwitchToBackgroundAsync();
        using FileStream stream = File.Create(file);
        await gachaLogService.ExportToUIGFAsync(archive, stream, useLegacyFormat, metadataContext).ConfigureAwait(false);

        await taskContext.SwitchToMainThreadAsync();
        messenger.Send(InfoBarMessage.Success(SH.FormatViewModelGachaLogUIGFExportSuccess(archive.Uid)));
    }

    private async ValueTask UpdateStatisticsAsync(GachaArchive? archive)
    {
        if (archive is not null)
        {
            ArgumentNullException.ThrowIfNull(metadataContext);
            GachaStatistics statistics = await gachaLogService.GetStatisticsAsync(metadataContext, archive).ConfigureAwait(false);
            await taskContext.SwitchToMainThreadAsync();
            Statistics = statistics;
            IsInitialized = true;
        }
        else
        {
            await taskContext.SwitchToMainThreadAsync();
            Statistics = default;
        }
    }
}
