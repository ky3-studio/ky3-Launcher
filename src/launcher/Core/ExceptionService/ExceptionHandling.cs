//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Launcher.Factory.Process;
using Launcher.UI.Xaml.View.Window;
using Launcher.Win32;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Launcher.Core.ExceptionService;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class ExceptionHandling
{
    private readonly ILogger<ExceptionHandling> logger;

    [GeneratedConstructor]
    public partial ExceptionHandling(IServiceProvider serviceProvider);

    public static void Initialize(IServiceProvider serviceProvider, Application app)
    {
        serviceProvider.GetRequiredService<ExceptionHandling>().Attach(app);
    }

    /// <summary>
    /// Kill the current process if the exception is or has a DbException.
    /// As this method does not throw, it should only be used in catch blocks
    /// </summary>
    /// <param name="exception">Incoming exception</param>
    /// <returns>Unwrapped DbException or original exception</returns>
    [StackTraceHidden]
    public static Exception KillProcessOnDbExceptionNoThrow(Exception exception)
    {
        return exception switch
        {
            DbException dbException => KillProcessOnDbException(dbException),
            DbUpdateException { InnerException: DbException dbException2 } => KillProcessOnDbException(dbException2),
            _ => exception,
        };
    }

    [StackTraceHidden]
    private static DbException KillProcessOnDbException(DbException exception)
    {
        LauncherNative.Instance.ShowErrorMessage("Warning | 警告", exception.Message);
        ProcessFactory.KillCurrent();
        return exception;
    }

    private static void OnAppUnhandledException(object? sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Exception? exception = e.Exception;

        if (exception is null)
        {
            return;
        }

        // WinUI 3 框架层已知异常（MUI 资源缓存/WinRT 文件未找到），非应用代码导致，忽略以防崩溃
        if (ExceptionFilter.IsKnownWinUiFrameworkException(exception))
        {
            e.Handled = true;
            string breadcrumb = exception is System.Runtime.InteropServices.COMException
                ? "WinUI 3 MUI 资源未加载 (0x80073B01)，已忽略"
                : "WinUI 3 WinRT 文件未找到 (0x80070002)，已忽略";
            SentrySdk.AddBreadcrumb(breadcrumb, level: BreadcrumbLevel.Warning);
            return;
        }

        Debugger.Break();
        XamlApplicationLifetime.Exiting = true;

        KillProcessOnDbExceptionNoThrow(e.Exception);

        // https://github.com/getsentry/sentry-dotnet/blob/main/src/Sentry/Integrations/WinUIUnhandledExceptionIntegration.cs
        exception.SetSentryMechanism("Microsoft.UI.Xaml.UnhandledException", handled: false);

        SentryId id = SentrySdk.CaptureException(e.Exception, scope =>
        {
            if (ExceptionAttachment.TryGetAttachment(e.Exception, out SentryAttachment? attachment))
            {
                scope.AddAttachment(attachment);
            }
        });
        SentrySdk.Flush();

        // Handled has to be set to true, the control flow is returned after post
        e.Handled = true;

        if (XamlApplicationLifetime.Exited)
        {
            return;
        }

        CapturedException capturedException = new(id, exception);

        if (SynchronizationContext.Current is not { } syncContext)
        {
            ProcessFactory.KillCurrent();
            return;
        }

#pragma warning disable SH007
        syncContext.Post(static state => ExceptionWindow.Show(Unsafe.Unbox<CapturedException>(state!)), capturedException);
#pragma warning restore SH007
    }

    private void Attach(Application app)
    {
        app.UnhandledException += OnAppUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        ConfigureDebugSettings(app);
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        // 设为已观察，防止 TaskExceptionHolder.Finalize 崩溃
        e.SetObserved();

        // 所有内层异常均为网络错误时静默处理（使用 All 确保每一个异常都在安全范围内）
        if (ExceptionFilter.IsAllSafeNetworkException(e.Exception))
        {
            return;
        }

        SentrySdk.CaptureException(e.Exception.Flatten());
    }

    [Conditional("DEBUG")]
    private void ConfigureDebugSettings(Application app)
    {
        app.DebugSettings.FailFastOnErrors = false;

        app.DebugSettings.IsBindingTracingEnabled = true;
        app.DebugSettings.BindingFailed += OnXamlBindingFailed;

        app.DebugSettings.IsXamlResourceReferenceTracingEnabled = true;
        app.DebugSettings.XamlResourceReferenceFailed += OnXamlResourceReferenceFailed;

        app.DebugSettings.LayoutCycleTracingLevel = LayoutCycleTracingLevel.High;
        app.DebugSettings.LayoutCycleDebugBreakLevel = LayoutCycleDebugBreakLevel.High;
    }

    private void OnXamlBindingFailed(object? sender, BindingFailedEventArgs e)
    {
        logger.LogCritical("XAML Binding Failed:{Message}", e.Message);
    }

    private void OnXamlResourceReferenceFailed(DebugSettings sender, XamlResourceReferenceFailedEventArgs e)
    {
        logger.LogCritical("XAML Resource Reference Failed:{Message}", e.Message);
    }
}
