//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Launcher.Core.Logging;
using Launcher.Factory.ContentDialog;
using Launcher.Factory.Picker;
using Launcher.Service.Notification;

namespace Launcher.ViewModel.Setting;

[BindableCustomPropertyProvider]
[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingStorageViewModel : Abstraction.ViewModel
{
    private readonly IFileSystemPickerInteraction fileSystemPickerInteraction;
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial SettingStorageViewModel(IServiceProvider serviceProvider);

    [ObservableProperty]
    public partial SettingFolderViewModel? CacheFolderView { get; set; }

    [ObservableProperty]
    public partial SettingFolderViewModel? DataFolderView { get; set; }

    [ObservableProperty]
    public partial SettingFolderViewModel? InstallFolderView { get; set; }

    [Command("SetDataFolderCommand")]
    private async Task SetDataFolderAsync()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Set data folder path", "SettingStorageViewModel.Command"));

        SettingStorageSetDataFolderOperation operation = new()
        {
            FileSystemPickerInteraction = fileSystemPickerInteraction,
            ContentDialogFactory = contentDialogFactory,
            Messenger = messenger,
        };

        if (await operation.TryExecuteAsync().ConfigureAwait(false))
        {
            messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingSetDataFolderSuccess));
        }
    }
}