//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core;
using kyxsan.Core.Setting;
using kyxsan.Web.kyxsan;
using kyxsan.Web.kyxsan.Response;
using kyxsan.Web.Response;

namespace kyxsan.Service.Update;

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
            try
            {
                ITaskContext taskContext = scope.ServiceProvider.GetRequiredService<ITaskContext>();
                await taskContext.SwitchToBackgroundAsync();

                kyxsanInfrastructureClient infrastructureClient = scope.ServiceProvider.GetRequiredService<kyxsanInfrastructureClient>();
                kyxsanResponse<kyxsanPackageInformation> response = await infrastructureClient.GetkyxsanVersionInformationAsync(token).ConfigureAwait(false);

                if (!ResponseValidator.TryValidateWithoutUINotification(response, scope.ServiceProvider, out kyxsanPackageInformation? packageInformation))
                {
                    checkUpdateResult.Kind = CheckUpdateResultKind.VersionApiInvalidResponse;
                    return checkUpdateResult;
                }

                checkUpdateResult.Kind = CheckUpdateResultKind.UpdateAvailable;
                checkUpdateResult.PackageInformation = packageInformation;

                if (!LocalSetting.Get(SettingKeys.OverrideUpdateVersionComparison, false))
                {
                    // Launched in an updated version
                    if (kyxsanRuntime.Version >= checkUpdateResult.PackageInformation.Version)
                    {
                        checkUpdateResult.Kind = CheckUpdateResultKind.AlreadyUpdated;
                        return checkUpdateResult;
                    }
                }

                return checkUpdateResult;
            }
            finally
            {
                UpdateInfo = checkUpdateResult.Kind switch
                {
                    CheckUpdateResultKind.UpdateAvailable => SH.FormatViewModelSettingUpdateAvailable(checkUpdateResult.PackageInformation?.Version.ToString()),
                    CheckUpdateResultKind.AlreadyUpdated => SH.ViewModelSettingAlreadyUpdated,
                    CheckUpdateResultKind.VersionApiInvalidResponse or CheckUpdateResultKind.VersionApiInvalidSha256 => SH.ViewModelSettingCheckUpdateFailed,
                    _ => default,
                };
            }
        }
    }

}