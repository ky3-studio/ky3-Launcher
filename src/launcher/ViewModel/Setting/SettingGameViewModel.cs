//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by Launcher.
// Licensed under the MIT license.

using Launcher.Core.Logging;
using Launcher.Service;
using Launcher.Service.Game;
using Launcher.Service.Notification;
using System.IO;

namespace Launcher.ViewModel.Setting;

[Service(ServiceLifetime.Scoped)]
internal sealed partial class SettingGameViewModel : Abstraction.ViewModel
{
    private readonly LaunchOptions launchOptions;
    private readonly IMessenger messenger;

    [GeneratedConstructor]
    public partial SettingGameViewModel(IServiceProvider serviceProvider);

    public partial AppOptions AppOptions { get; }

    [Command("DeleteGameWebCacheCommand")]
    private void DeleteGameWebCache()
    {
        SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateUI("Delete game web cache", "SettingGameViewModel.Command"));

        string? gamePath = launchOptions.GamePathEntry.Value?.Path;

        if (string.IsNullOrEmpty(gamePath))
        {
            messenger.Send(InfoBarMessage.Warning(SH.FormatViewModelSettingClearWebCachePathInvalid(string.Empty)));
            return;
        }

        string cacheFolder = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(gamePath)!, @"..\webCaches"));

        if (!Directory.Exists(cacheFolder))
        {
            messenger.Send(InfoBarMessage.Warning(SH.FormatViewModelSettingClearWebCachePathInvalid(cacheFolder)));
            return;
        }

        try
        {
            Directory.Delete(cacheFolder, true);
            messenger.Send(InfoBarMessage.Success(SH.ViewModelSettingClearWebCacheSuccess));
        }
        catch (UnauthorizedAccessException)
        {
            messenger.Send(InfoBarMessage.Warning(SH.ViewModelSettingClearWebCacheFail));
        }
    }
}