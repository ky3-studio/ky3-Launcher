//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.DataTransfer;
using kyxsan.Factory.ContentDialog;
using kyxsan.Factory.Picker;
using kyxsan.Service.Yae;

namespace kyxsan.ViewModel.Achievement;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class AchievementImporterScopeContext
{
    [GeneratedConstructor]
    public partial AchievementImporterScopeContext(IServiceProvider serviceProvider);

    public partial IFileSystemPickerInteraction FileSystemPickerInteraction { get; }

    public partial JsonSerializerOptions JsonSerializerOptions { get; }

    public partial IContentDialogFactory ContentDialogFactory { get; }

    public partial IClipboardProvider ClipboardProvider { get; }

    public partial IServiceProvider ServiceProvider { get; }

    public partial IYaeService YaeService { get; }

    public partial IMessenger Messenger { get; }
}