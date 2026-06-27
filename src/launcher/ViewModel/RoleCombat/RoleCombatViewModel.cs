//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Launcher.Core.Logging;
using Launcher.Service.Metadata;
using Launcher.Service.Metadata.ContextAbstraction;
using Launcher.Service.Notification;
using Launcher.Service.RoleCombat;
using Launcher.Service.User;
using Launcher.UI.Xaml.Data;
using Launcher.ViewModel.Complex;
using Launcher.ViewModel.User;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Launcher.RoleCombat;
using System.Collections.ObjectModel;

namespace Launcher.ViewModel.RoleCombat;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class RoleCombatViewModel : Abstraction.ViewModel, IRecipient<UserAndUidChangedMessage>
{
    private readonly IRoleCombatService roleCombatService;
    private readonly IServiceProvider serviceProvider;
    private readonly IMetadataService metadataService;
    private readonly ITaskContext taskContext;
    private readonly IUserService userService;
    private readonly IMessenger messenger;

    private RoleCombatMetadataContext? metadataContext;

    [GeneratedConstructor]
    public partial RoleCombatViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial IAdvancedCollectionView<RoleCombatView>? RoleCombatEntries { get; set; }

    public partial LauncherRoleCombatDatabaseViewModel LauncherRoleCombatDatabaseViewModel { get; }

    public void Receive(UserAndUidChangedMessage message)
    {
        if (message.UserAndUid is { } userAndUid)
        {
            UpdateRoleCombatCollectionAsync(userAndUid).SafeForget();
        }
        else
        {
            RoleCombatEntries?.MoveCurrentTo(default);
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
            metadataContext = await metadataService.GetContextAsync<RoleCombatMetadataContext>(token).ConfigureAwait(false);
            await UpdateRoleCombatCollectionAsync(userAndUid).ConfigureAwait(false);
        }
        else
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
        }

        return true;
    }

    [SuppressMessage("", "SH003")]
    private async Task UpdateRoleCombatCollectionAsync(UserAndUid userAndUid)
    {
        if (metadataContext is null)
        {
            return;
        }

        try
        {
            ObservableCollection<RoleCombatView> collection;
            using (await EnterCriticalSectionAsync().ConfigureAwait(false))
            {
                collection = await roleCombatService
                    .GetRoleCombatViewCollectionAsync(metadataContext, userAndUid)
                    .ConfigureAwait(false);
            }

            IAdvancedCollectionView<RoleCombatView> roleCombatEntries = collection.AsAdvancedCollectionView();

            await taskContext.SwitchToMainThreadAsync();
            RoleCombatEntries = roleCombatEntries;
            RoleCombatEntries.MoveCurrentTo(RoleCombatEntries.Source.FirstOrDefault(s => s.Engaged) ?? RoleCombatEntries.Source.FirstOrDefault());
        }
        catch (OperationCanceledException)
        {
        }
    }

    [Command("RefreshCommand")]
    private async Task RefreshAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Refresh role combat record", "RoleCombatViewModel.Command"));

        if (metadataContext is null)
        {
            return;
        }

        if (RoleCombatEntries is not null)
        {
            if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is { } userAndUid)
            {
                try
                {
                    using (await EnterCriticalSectionAsync().ConfigureAwait(false))
                    {
                        await roleCombatService
                            .RefreshRoleCombatAsync(metadataContext, userAndUid)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                }

                await taskContext.SwitchToMainThreadAsync();
                RoleCombatEntries.MoveCurrentTo(RoleCombatEntries.Source.FirstOrDefault(s => s.Engaged) ?? RoleCombatEntries.Source.FirstOrDefault());
            }
        }
    }

    [Command("UploadRoleCombatRecordCommand")]
    private async Task UploadRoleCombatRecordAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Upload role combat record", "RoleCombatViewModel.Command"));

        if (await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false) is not { } userAndUid)
        {
            messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            return;
        }

        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            LauncherRoleCombatClient roleCombatClient = scope.ServiceProvider.GetRequiredService<LauncherRoleCombatClient>();
            if (await roleCombatClient.GetPlayerRecordAsync(userAndUid).ConfigureAwait(false) is { } record)
            {
                LauncherResponse response = await roleCombatClient.UploadRecordAsync(record).ConfigureAwait(false);

                messenger.Send(InfoBarMessage.Any(
                    response is { ReturnCode: 0 } ? InfoBarSeverity.Success : InfoBarSeverity.Warning,
                    response.GetLocalizationMessageOrMessage()));
            }
            else
            {
                messenger.Send(InfoBarMessage.Warning(SH.MustSelectUserAndUid));
            }
        }
    }
}