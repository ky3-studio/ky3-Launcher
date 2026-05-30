//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.Globalization;
using Quartz;
using kyxsan.Core.Logging;
using kyxsan.Service;
using kyxsan.Service.Notification;
using kyxsan.Web.Response;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.Core.DependencyInjection;

internal static class DependencyInjection
{
    public static ServiceProvider Initialize()
    {
        IServiceCollection services = new ServiceCollection()
            .AddLogging(builder =>
            {
                builder
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddFilter(DbLoggerCategory.Database.Command.Name, level => level >= LogLevel.Information)
                    .AddFilter(DbLoggerCategory.Query.Name, level => level >= LogLevel.Information)
                    .AddDebug()
                    .AddSentryTelemetry();
            })
            .AddMemoryCache()
            .AddQuartz()
            .AddJsonOptions()
            .AddDatabase()
            .AddServices()
            .AddThirdPartyToolService()
            .AddResponseValidation()
            .AddConfiguredHttpClients()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddSingleton<AutoStartService>();

        ServiceProvider serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
#if DEBUG
            ValidateOnBuild = true,
            ValidateScopes = true,
#endif
        });

        Ioc.Default.ConfigureServices(serviceProvider);

        serviceProvider.InitializeCulture();
        serviceProvider.InitializeNotification();

        return serviceProvider;
    }

    extension(IServiceProvider serviceProvider)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeCulture()
        {
            CultureOptions cultureOptions = serviceProvider.GetRequiredService<CultureOptions>();
            cultureOptions.SystemCulture = CultureInfo.CurrentCulture;

            CultureInfo cultureInfo = cultureOptions.CurrentCulture.Value;

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;

            try
            {
                ApplicationLanguages.PrimaryLanguageOverride = cultureInfo.Name;
            }
            catch (COMException)
            {
            }

            SH.Culture = cultureInfo;
            XamlApplicationLifetime.CultureInfoInitialized = true;

            ILogger<CultureOptions> logger = serviceProvider.GetRequiredService<ILogger<CultureOptions>>();
            logger.LogDebug("System Culture: {System}", cultureOptions.SystemCulture);
            logger.LogDebug("Current Culture: {Current}", cultureInfo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InitializeNotification()
        {
            _ = serviceProvider.GetRequiredService<IAppNotificationLifeTime>();
        }
    }
}