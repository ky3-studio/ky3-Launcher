//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.LifeCycle.InterProcess.Model;
using kyxsan.Core.Security.Principal;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;

namespace kyxsan.Core.LifeCycle.InterProcess;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class PrivateNamedPipeServer : IDisposable
{
    private readonly PrivateNamedPipeMessageDispatcher messageDispatcher;
    private readonly ILogger<PrivateNamedPipeServer> logger;

    private readonly NamedPipeServerStream serverStream = CreatePipeServerStream();
    private readonly CancellationTokenSource serverTokenSource = new();
    private readonly AsyncLock serverLock = new();

    [GeneratedConstructor]
    public partial PrivateNamedPipeServer(IServiceProvider serviceProvider);

    public void Dispose()
    {
        serverTokenSource.Cancel();
        using AsyncLock.Releaser discard = serverLock.LockAsync().GetAwaiter().GetResult();
        serverTokenSource.Dispose();
        serverStream.Dispose();
    }

    public void Start()
    {
        RunAsync().SafeForget();
    }

    private static NamedPipeServerStream CreatePipeServerStream()
    {
        PipeSecurity pipeSecurity = new();
        pipeSecurity.AddAccessRule(new(SecurityIdentifiers.Everyone, PipeAccessRights.FullControl, AccessControlType.Allow));

        return NamedPipeServerStreamAcl.Create(
            PrivateNamedPipe.PrivateName,
            PipeDirection.InOut,
            NamedPipeServerStream.MaxAllowedServerInstances,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous | PipeOptions.WriteThrough,
            0,
            0,
            pipeSecurity);
    }

    private async ValueTask RunAsync()
    {
        using (await serverLock.LockAsync().ConfigureAwait(false))
        {
            while (!serverTokenSource.IsCancellationRequested)
            {
                try
                {
                    await serverStream.WaitForConnectionAsync(serverTokenSource.Token).ConfigureAwait(false);
                    logger.LogInformation("Pipe session created");
                    RunPacketSession(serverStream, serverTokenSource.Token);
                }
                catch (IOException)
                {
                    try
                    {
                        serverStream.Disconnect();
                    }
                    catch
                    {
                        // Ignored
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }
    }

    private void RunPacketSession(NamedPipeServerStream serverStream, CancellationToken token)
    {
        while (serverStream.IsConnected && !token.IsCancellationRequested)
        {
            serverStream.ReadPacket(out PipePacketHeader header);
            logger.LogInformation("Pipe packet: [Type:{Type}] [Command:{Command}]", header.Type, header.Command);
            switch (header.Type, header.Command)
            {
                case (PipePacketType.Request, PipePacketCommand.RequestElevationStatus):
                    ElevationStatusResponse resp = new(kyxsanRuntime.IsProcessElevated, Environment.ProcessId);
                    serverStream.WritePacketWithJsonContent(PrivateNamedPipe.PrivateVersion, PipePacketType.Response, PipePacketCommand.ResponseElevationStatus, resp);
                    serverStream.Flush();
                    break;

                case (PipePacketType.Request, PipePacketCommand.RedirectActivation):
                    kyxsanActivationArguments? kyxsanArgs = serverStream.ReadJsonContent<kyxsanActivationArguments>(in header);
                    if (kyxsanArgs is not null)
                    {
                        logger.LogInformation("Redirect activation: [Kind:{Kind}] [Arguments:{Arguments}]", kyxsanArgs.Kind, kyxsanArgs.LaunchActivatedArguments);
                    }

                    messageDispatcher.RedirectedActivation(kyxsanArgs);
                    break;

                case (PipePacketType.SessionTermination, _):
                    serverStream.Disconnect();
                    if (header.Command is PipePacketCommand.Exit)
                    {
                        messageDispatcher.ExitApplication();
                    }

                    return;
            }
        }
    }
}