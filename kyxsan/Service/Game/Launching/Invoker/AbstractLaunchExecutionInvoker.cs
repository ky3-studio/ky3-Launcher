//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Diagnostics;
using kyxsan.Core.ExceptionService;
using kyxsan.Factory.Progress;
using kyxsan.Factory.Process;
using kyxsan.Service.Game.FileSystem;
using kyxsan.Service.Game.Launching.Context;
using kyxsan.Service.Game.Launching.Handler;
using kyxsan.Service.Game.Package;
using System.Collections.Concurrent;
using System.Collections.Immutable;

namespace kyxsan.Service.Game.Launching.Invoker;

internal abstract class AbstractLaunchExecutionInvoker
{
    private static readonly ConcurrentDictionary<AbstractLaunchExecutionInvoker, Void> Invokers = [];

    private bool invoked;

    protected ImmutableArray<ILaunchExecutionHandler> Handlers { get; init; }

    public static bool Invoking()
    {
        return !Invokers.IsEmpty;
    }

    public async ValueTask InvokeAsync(LaunchExecutionInvocationContext context)
    {
        kyxsanException.ThrowIf(Interlocked.Exchange(ref invoked, true), "The invoker has been invoked");
        ITaskContext taskContext = context.ServiceProvider.GetRequiredService<ITaskContext>();

        try
        {
            Invokers.TryAdd(this, default);
            await InvokeCoreAsync(context, taskContext).ConfigureAwait(false);
        }
        finally
        {
            Invokers.TryRemove(this, out _);
            if (!Invoking())
            {
                await GameLifeCycle.SpinWaitGameExitAsync(taskContext).ConfigureAwait(false);
            }
        }
    }

    protected virtual IProcess? CreateProcess(BeforeLaunchExecutionContext beforeContext)
    {
        return GameProcessFactory.CreateForDefault(beforeContext);
    }

    private static IProgress<LaunchStatus?> CreateStatusProgress(IServiceProvider serviceProvider)
    {
        IProgressFactory progressFactory = serviceProvider.GetRequiredService<IProgressFactory>();
        LaunchStatusOptions options = serviceProvider.GetRequiredService<LaunchStatusOptions>();
        return progressFactory.CreateForMainThread<LaunchStatus?, LaunchStatusOptions>(static (status, options) => options.LaunchStatus = status, options);
    }

    private async ValueTask InvokeCoreAsync(LaunchExecutionInvocationContext context, ITaskContext taskContext)
    {
        string lockTrace = $"{GetType().Name}.{nameof(InvokeAsync)}";
        context.LaunchOptions.TryGetGameFileSystem(lockTrace, out IGameFileSystem? gameFileSystem);
        ArgumentNullException.ThrowIfNull(gameFileSystem);

        using (GameFileSystemReference fileSystemReference = new(gameFileSystem))
        {
            if (context.ViewModel.TargetScheme is not { } targetScheme)
            {
                throw kyxsanException.InvalidOperation(SH.ViewModelLaunchGameSchemeNotSelected);
            }

            if (context.ViewModel.CurrentScheme is not { } currentScheme)
            {
                throw kyxsanException.InvalidOperation(SH.ServiceGameLaunchExecutionCurrentSchemeNull);
            }

            IProgress<LaunchStatus?> progress = CreateStatusProgress(context.ServiceProvider);

            BeforeLaunchExecutionContext beforeContext = new()
            {
                ViewModel = context.ViewModel,
                Progress = progress,
                ServiceProvider = context.ServiceProvider,
                TaskContext = taskContext,
                FileSystem = fileSystemReference,
                HoyoPlay = context.ServiceProvider.GetRequiredService<IHoyoPlayService>(),
                Messenger = context.ServiceProvider.GetRequiredService<IMessenger>(),
                LaunchOptions = context.LaunchOptions,
                CurrentScheme = currentScheme,
                TargetScheme = targetScheme,
                Identity = context.Identity,
            };

            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.BeforeAsync(beforeContext).ConfigureAwait(false);
            }

            fileSystemReference.Exchange(beforeContext.FileSystem);

            // 始终创建游戏进程，DLL 注入会在游戏启动后进行
            IProcess? process = CreateProcess(beforeContext);

            using (process)
            {
                if (process is null)
                {
                    return;
                }

                LaunchExecutionContext executionContext = new()
                {
                    Progress = progress,
                    ServiceProvider = context.ServiceProvider,
                    TaskContext = taskContext,
                    Messenger = context.ServiceProvider.GetRequiredService<IMessenger>(),
                    LaunchOptions = context.LaunchOptions,
                    Process = process ?? new NullProcess(),
                    IsOversea = targetScheme.IsOversea,
                };

                foreach (ILaunchExecutionHandler handler in Handlers)
                {
                    await handler.ExecuteAsync(executionContext).ConfigureAwait(false);
                }

                // 等待游戏进程退出
                if (process is { IsRunning: true })
                {
                    progress.Report(new(SH.ServiceGameLaunchPhaseWaitingProcessExit));
                    try
                    {
                        await taskContext.SwitchToBackgroundAsync();
                        process.WaitForExit();
                    }
                    catch (Exception ex)
                    {
                        SentrySdk.CaptureException(ex);
                        return;
                    }
                }
            }

            progress.Report(new(SH.ServiceGameLaunchPhaseProcessExited));

            AfterLaunchExecutionContext afterContext = new()
            {
                ServiceProvider = context.ServiceProvider,
                TaskContext = taskContext,
            };

            foreach (ILaunchExecutionHandler handler in Handlers)
            {
                await handler.AfterAsync(afterContext).ConfigureAwait(false);
            }
        }
    }
}