//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
// Copyright (c) Millennium-Science-Technology-R-D-Inst. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml;
using kyxsan.Core;
using kyxsan.Core.Logging;
using kyxsan.Core.Security.Principal;
using kyxsan.Core.Setting;
using kyxsan.Core.Shell;
using kyxsan.Service.Game.Launching;
using kyxsan.Win32;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using WinRT;

[assembly: DisableRuntimeMarshalling]

namespace kyxsan;

[SuppressMessage("", "SH001")]
public static partial class Bootstrap
{
    private const string LockName = "KYXSAN_BOOTSTRAP_LOCK";
    private static readonly ApplicationInitializationCallback AppInitializationCallback = InitializeApp;
    private static Mutex? mutex;

    internal static void UseNamedPipeRedirection()
    {
        if (mutex is null)
        {
            return;
        }
        DisposableMarshal.DisposeAndClear(ref mutex);
    }

    [STAThread]
    private static void Main(string[] args)
    {
        if (args.Length > 0 && string.Equals(args[0], "--elevated-inject", StringComparison.OrdinalIgnoreCase))
        {
            Environment.Exit(GameProcessFactory.RunElevatedInjection(args));
            return;
        }

        try
        {
            if (!kyxsanRuntime.IsProcessElevated)
            {
                bool runElevated = false;
                try
                {
                    runElevated = LocalSetting.Get(SettingKeys.RunElevated, false);
                }
                catch
                {
                }

                if (runElevated)
                {
                    SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateInfo("Requesting elevation based on user setting", "Startup"));
                    try
                    {
                        if (NativeMethods.RestartAsAdministratorAtStart())
                        {
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                    }
                }
            }
        }
        catch
        {
        }

        if (Mutex.TryOpenExisting(LockName, out _))
        {
            return;
        }

        try
        {
            MutexSecurity mutexSecurity = new();
            mutexSecurity.AddAccessRule(new(SecurityIdentifiers.Everyone, MutexRights.FullControl, AccessControlType.Allow));
            mutex = MutexAcl.Create(true, LockName, out bool created, mutexSecurity);
            Debug.Assert(created);
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            return;
        }

        using (mutex)
        {
            if (!OSPlatformSupported())
            {
                return;
            }

            Environment.SetEnvironmentVariable("WEBVIEW2_DEFAULT_BACKGROUND_COLOR", "00000000");
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_BUFFERS_SHAREDARRAYPOOL_MAXARRAYSPERPARTITION", "128");
            AppContext.SetData("MVVMTOOLKIT_ENABLE_INOTIFYPROPERTYCHANGING_SUPPORT", false);

            ComWrappersSupport.InitializeComWrappers();

            using (ServiceProvider serviceProvider = DependencyInjection.Initialize())
            {
                Thread.CurrentThread.Name = "ky3 Launcher Application Main Thread";

                Application.Start(AppInitializationCallback);

                XamlApplicationLifetime.Exited = true;
            }

            SentrySdk.Flush();
        }

    }

    private static void InitializeApp(ApplicationInitializationCallbackParams param)
    {
        Gen2GcCallback.Register(() =>
        {
            SentrySdk.AddBreadcrumb(BreadcrumbFactory.CreateDebug("Gen2 GC triggered.", "Runtime"));
            return true;
        });

        IServiceProvider serviceProvider = Ioc.Default;
        _ = serviceProvider.GetRequiredService<App>();
    }

    private static bool OSPlatformSupported()
    {
        if (!kyxsanNative.Instance.IsCurrentWindowsVersionSupported())
        {
            const string Message = """
                ky3 Launcher 无法在版本低于 10.0.19045.5371 的 Windows 上运行，请更新系统。
                ky3 Launcher cannot run on Windows versions earlier than 10.0.19045.5371. Please update your system.
                """;
            kyxsanNative.Instance.ShowErrorMessage("Warning | 警告", Message);
            return false;
        }

        return true;
    }
}