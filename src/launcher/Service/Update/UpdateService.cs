//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core;
using Launcher.Core.Setting;
using Launcher.Web.Launcher;
using Launcher.Web.Launcher.Response;
using Launcher.Web.Response;

namespace Launcher.Service.Update;

[Service(ServiceLifetime.Singleton, typeof(IUpdateService))]
internal sealed partial class UpdateService : IUpdateService
{
    // Avoid injecting services directly
    private readonly IServiceProvider serviceProvider;

    [GeneratedConstructor]
    public partial UpdateService(IServiceProvider serviceProvider);

    public string? UpdateInfo { get; set; }

    public async ValueTask<CheckUpdateResult> CheckUpdateAsync(CancellationToken token = default)
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            CheckUpdateResult checkUpdateResult = new();

            ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
            await taskContext.SwitchToBackgroundAsync();

            LauncherInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<LauncherInfrastructureClient>();
            LauncherResponse<LauncherPackageInformation> response = await infrastructureClient.GetLauncherVersionInformationAsync(token).ConfigureAwait(false);

            if (!ResponseValidator.TryValidateWithoutUINotification(response, scope.ServiceProvider, out LauncherPackageInformation? packageInformation))
            {
                checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                UpdateInfo = SH.ViewModelSettingCheckUpdateFailed;
                return checkUpdateResult;
            }

            checkUpdateResult.Kind = CheckUpdateResultKind.UpdateAvailable;
            checkUpdateResult.PackageInformation = packageInformation;

            if (!LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false))
            {
                // Launched in an updated version
                if (LauncherRuntime.Version >= checkUpdateResult.PackageInformation.Version)
                {
                    checkUpdateResult.Kind = CheckUpdateResultKind.AlreadyUpdated;
                    UpdateInfo = SH.ViewModelSettingAlreadyUpdated;
                    return checkUpdateResult;
                }
            }

            UpdateInfo = SH.FormatViewModelSettingUpdateAvailable(checkUpdateResult.PackageInformation.Version.ToString());
            return checkUpdateResult;
        }
    }

}
