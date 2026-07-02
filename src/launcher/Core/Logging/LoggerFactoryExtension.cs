//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Launcher.Core.ExceptionService;
using Launcher.Core.IO.Http.Proxy;
using Launcher.Win32;
using System.Runtime.CompilerServices;

namespace Launcher.Core.Logging;

internal static class LoggerFactoryExtension
{
    extension(ILoggingBuilder builder)
    {
        public ILoggingBuilder AddSentryTelemetry()
        {
            return builder.AddSentry(options =>
            {
                options.HttpProxy = HttpProxyUsingSystemProxy.Instance;

#if DEBUG || IS_ALPHA_BUILD || IS_CANARY_BUILD
                // Alpha and Canary produces noisy events
                options.Dsn = "https://cf990156535580d59562164163587599@o4511476524974080.ingest.us.sentry.io/4511476529233920";
#else
                options.Dsn = "https://cf990156535580d59562164163587599@o4511476524974080.ingest.us.sentry.io/4511476529233920";
#endif

#if DEBUG
                options.Debug = true;
#endif

                options.AutoSessionTracking = true;
                options.IsGlobalModeEnabled = true;
                options.EnableBackpressureHandling = true;
                options.Release = $"{LauncherRuntime.Version}";
                options.Environment = GetBuildEnvironment();

                // Suppress logs to generate events and breadcrumbs
                options.MinimumBreadcrumbLevel = LogLevel.Information;
                options.MinimumEventLevel = LogLevel.Error;

                options.ProfilesSampleRate = 1.0D;
                options.TracesSampleRate = 1.0D;

                // Use our own exception handling
                options.DisableWinUiUnhandledExceptionIntegration();

                options.ConfigureScope(scope =>
                {
                    scope.User = new()
                    {
                        Id = LauncherRuntime.DeviceId,
                    };

                    scope.SetTag("elevated", LauncherRuntime.IsProcessElevated ? "yes" : "no");
                    scope.SetWebView2Version();
                });

                options.AddExceptionProcessor(new SentryExceptionProcessor());

                options.SetBeforeSend(@event =>
                {
                    Sentry.Protocol.OperatingSystem operatingSystem = @event.Contexts.OperatingSystem;
                    LauncherPrivateWindowsVersion windowsVersion = LauncherNative.Instance.GetCurrentWindowsVersion();
                    operatingSystem.Build = $"{windowsVersion.Build}";
                    operatingSystem.Name = "Windows";
                    operatingSystem.Version = $"{windowsVersion}";

                    return @event;
                });
            });
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string GetBuildEnvironment()
    {
#if DEBUG
        return "DEBUG";
#elif IS_ALPHA_BUILD
        return "ALPHA";
#elif IS_CANARY_BUILD
        return "CANARY";
#else
        return "RELEASE";
#endif
    }

    extension(Scope scope)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetWebView2Version()
        {
            WebView2Version webView2Version = LauncherRuntime.WebView2Version;
            Dictionary<string, object> webView2 = new()
            {
                ["Supported"] = webView2Version.Supported,
                ["Version"] = webView2Version.RawVersion,
            };

            scope.Contexts["WebView2"] = webView2;
        }
    }
}
