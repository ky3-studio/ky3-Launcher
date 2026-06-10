//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Logging;
using kyxsan.Factory.ContentDialog;
using kyxsan.Service;
using kyxsan.Service.Notification;
using kyxsan.UI.Xaml.View.Dialog;

namespace kyxsan.ViewModel.Setting;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingGeetestViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly ITaskContext taskContext;
    private readonly AppOptions appOptions;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial SettingGeetestViewModel(IServiceProvider serviceProvider);

    [Command("ConfigureGeetestUrlCommand")]
    private async Task ConfigureGeetestUrlAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Config geetest url", "SettingGeetestViewModel.Command"));

        GeetestCustomUrlDialog dialog = await contentDialogFactory.CreateInstanceAsync<GeetestCustomUrlDialog>(serviceProvider).ConfigureAwait(false);
        (bool isOk, string template) = await dialog.GetUrlAsync().ConfigureAwait(false);

        if (!isOk)
        {
            return;
        }

        await taskContext.SwitchToMainThreadAsync();
        appOptions.GeetestCustomCompositeUrl.Value = template;
        messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingGeetestCustomUrlSucceed));
    }
}