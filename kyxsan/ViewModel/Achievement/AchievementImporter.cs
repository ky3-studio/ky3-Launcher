//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using kyxsan.Core.IO;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Model.InterChange.Achievement;
using kyxsan.Service.Achievement;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.View.Dialog;
using EntityAchievementArchive = kyxsan.Model.Entity.AchievementArchive;

namespace kyxsan.ViewModel.Achievement;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class AchievementImporter
{
    private readonly AchievementImporterScopeContext scopeContext;

    [GeneratedConstructor]
    public partial AchievementImporter(IServiceProvider serviceProvider);

    public async ValueTask<bool> FromClipboardAsync(AchievementViewModelScopeContext context)
    {
        if (await context.AchievementService.GetArchiveCollectionAsync().ConfigureAwait(false) is not { CurrentItem: { } archive })
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2));
            return false;
        }

        UIAF? uiaf;
        try
        {
            uiaf = await scopeContext.ClipboardProvider.DeserializeFromJsonAsync<UIAF>().ConfigureAwait(false);
        }
        catch
        {
            uiaf = null;
        }

        if (uiaf is null)
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ViewModelImportFromClipboardErrorTitle, SH.ViewModelImportFromClipboardErrorMessage));
            return false;
        }

        return await TryImportAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    public async ValueTask<bool> FromEmbeddedYaeAsync(AchievementViewModelScopeContext context)
    {
        if (await context.AchievementService.GetArchiveCollectionAsync().ConfigureAwait(false) is not { CurrentItem: { } archive })
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2));
            return false;
        }

        if (await scopeContext.YaeService.GetAchievementAsync().ConfigureAwait(false) is not { } uiaf)
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ServiceYaeEmbeddedYaeErrorTitle, SH.ViewModelImportByEmbeddedYaeErrorMessage));
            return false;
        }

        return await TryImportAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    public async ValueTask<bool> FromFileAsync(AchievementViewModelScopeContext context)
    {
        if (await context.AchievementService.GetArchiveCollectionAsync().ConfigureAwait(false) is not { CurrentItem: { } archive })
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage2));
            return false;
        }

        if (scopeContext.FileSystemPickerInteraction.PickFile(SH.ServiceAchievementUIAFImportPickerTitile, SH.ServiceAchievementUIAFImportPickerFilterText, "*.json") is not (true, { HasValue: true } file))
        {
            return false;
        }

        if (await file.DeserializeFromJsonNoThrowAsync<UIAF>(scopeContext.JsonSerializerOptions).ConfigureAwait(false) is not (true, { } uiaf))
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelImportWarningMessage));
            return false;
        }

        return await TryImportAsync(context, archive, uiaf).ConfigureAwait(false);
    }

    private async ValueTask<bool> TryImportAsync(AchievementViewModelScopeContext context, EntityAchievementArchive archive, UIAF uiaf)
    {
        if (!uiaf.IsCurrentVersionSupported())
        {
            scopeContext.Messenger.Send(InfoBarMessage.Warning(SH.ViewModelImportWarningTitle, SH.ViewModelAchievementImportWarningMessage));
            return false;
        }

        AchievementImportDialog importDialog = await scopeContext.ContentDialogFactory.CreateInstanceAsync<AchievementImportDialog>(scopeContext.ServiceProvider, uiaf).ConfigureAwait(false);
        if (await importDialog.GetImportStrategyAsync().ConfigureAwait(false) is not (true, var strategy))
        {
            return false;
        }

        ContentDialog dialog = await scopeContext.ContentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ViewModelAchievementImportProgress)
            .ConfigureAwait(false);

        using (await scopeContext.ContentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            ImportResult result = await context.AchievementService.ImportFromUIAFAsync(archive, uiaf.List, strategy).ConfigureAwait(false);
            scopeContext.Messenger.Send(InfoBarMessage.Success(SH.FormatServiceAchievementImportResult(result.Add, result.Update, result.Remove)));
        }

        return true;
    }
}