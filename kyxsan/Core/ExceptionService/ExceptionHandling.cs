//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using kyxsan.Factory.Process;
using kyxsan.UI.Xaml.View.Window;
using kyxsan.Win32;
using System.Data.Common;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.ExceptionService;

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
        kyxsanNative.Instance.ShowErrorMessage("Warning | 警告", exception.Message);
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
        ConfigureDebugSettings(app);
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