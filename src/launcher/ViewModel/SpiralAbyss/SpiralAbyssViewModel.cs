//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using kyxsan.Core.Logging;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service.kyxsan;
using kyxsan.Service.Metadata;
using kyxsan.Service.Metadata.ContextAbstraction;
using kyxsan.Service.Navigation;
using kyxsan.Service.Notification;
using kyxsan.Service.SpiralAbyss;
using kyxsan.Service.User;
using kyxsan.UI.Xaml.Data;
using kyxsan.UI.Xaml.View.Dialog;
using kyxsan.UI.Xaml.View.Page;
using kyxsan.ViewModel.Complex;
using kyxsan.ViewModel.User;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.kyxsan.SpiralAbyss;
using System.Collections.ObjectModel;

namespace kyxsan.ViewModel.SpiralAbyss;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SpiralAbyssViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly ISpiralAbyssService spiralAbyssService;
    private readonly INavigationService navigationService;
    private readonly kyxsanUserOptions kyxsanUserOptions;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private SpiralAbyssMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial SpiralAbyssViewModel(IServiceProvider serviceProvider);

    public IAdvancedCollectionView<SpiralAbyssView>? SpiralAbyssEntries
    {
        get;
        set
        {
            AdvancedCollectionViewCurrentChanged.Detach(field, OnCurrentSpiralAbyssEntryChanged);
            SetProperty(ref field, value);
            AdvancedCollectionViewCurrentChanged.Attach(field, OnCurrentSpiralAbyssEntryChanged);
        }
    }

    public partial kyxsanSpiralAbyssDatabaseViewModel kyxsanSpiralAbyssDatabaseViewModel { get; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateSpiralAbyssCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            SpiralAbyssEntries?.MoveCurrentTo(default);
        }
    }

    protected override async ValueTask<bool> LoadOverrideAsync(CancellationToken token)
    {
        if (!await metadataService.InitializeAsync().ConfigureAwait(false))
        {
            return false;
        }

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
        {
            metadataContext = await metadataService.GetContextAsync<SpiralAbyssMetadataContext>(token).ConfigureAwait(false);
            await UpdateSpiralAbyssCollectionAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        return true;
    }

    private void OnCurrentSpiralAbyssEntryChanged(object? sender, object? e)
    {
        SpiralAbyssEntries?.CurrentItem?.Floors.MoveCurrentToFirst();
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateSpiralAbyssCollectionAsync(UserAndUid userAndUid)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            ObservableCollection<SpiralAbyssView> collection;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                collection = await spiralAbyssService
                    .GetSpiralAbyssViewCollectionAsync(metadataContext, userAndUid)
                    .ConfigureAwait(false);
            }

            IAdvancedCollectionView<SpiralAbyssView> spiralAbyssEntries = collection.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            SpiralAbyssEntries = spiralAbyssEntries;
            SpiralAbyssEntries.MoveCurrentTo(SpiralAbyssEntries.Source.FirstOrDefault(s => s.Engaged));
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh spiral abyss record", "SpiralAbyssRecordViewModel.Command"));

        if (metadataContext is null)
        {
            return;
        }

        if (SpiralAbyssEntries is not null)
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        await spiralAbyssService
                            .RefreshSpiralAbyssAsync(metadataContext, userAndUid)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await taskContext.SwitchToMainThreadAsync();
                SpiralAbyssEntries.MoveCurrentTo(SpiralAbyssEntries.Source.FirstOrDefault(s => s.Engaged));
            }
        }
    }

    [Command("UploadSpiralAbyssRecordCommand")]
    private async Task UploadSpiralAbyssRecordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload spiral abyss record", "SpiralAbyssRecordViewModel.Command"));

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        if (!kyxsanUserOptions.IsLoggedIn)
        {
            SpiralAbyssUploadRecordHomaNotLoginDialog dialog = await contentDialogFactory
                .CreateInstanceAsync<SpiralAbyssUploadRecordHomaNotLoginDialog>(serviceProvider)
                .ConfigureAwait(false);

            ContentDialogResult result = await contentDialogFactory.EnqueueAndShowAsync(dialog).ShowTask.ConfigureAwait(false);

            switch (result)
            {
                case ContentDialogResult.Primary:
                    await navigationService.NavigateAsync<kyxsanPassportPage>(NavigationExtraData.Default, true).ConfigureAwait(false);
                    return;

                case ContentDialogResult.Secondary:
                    break;

                case ContentDialogResult.None:
                    return;
            }
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            kyxsanSpiralAbyssClient spiralAbyssClient = scope.ServiceProvider.GetRequiredService<kyxsanSpiralAbyssClient>();
            if (await spiralAbyssClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false) is { } record)
            {
                kyxsanResponse response = await spiralAbyssClient.UploadRecordAsync(record).ConfigureAwait(false);

                if (response is ILocalizableResponse localizableResponse)
                {
                    messenger.Send(InfoBarMessage.Any(
                        response is { ReturnCode: 0 } ? InfoBarSeverity.Success : InfoBarSeverity.Warning,
                        localizableResponse.GetLocalizationMessage()));
                }
            }
            else
            {
                messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            }
        }
    }
}