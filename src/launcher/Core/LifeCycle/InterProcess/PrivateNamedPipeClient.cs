//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by ky3-studio.
// Licensed under the MIT license.

using Microsoft.Windows.AppLifecycle;
using Launcher.Core.Diagnostics;
using Launcher.Core.LifeCycle.InterProcess.Model;
using Launcher.Factory.Process;
using System.IO;
using System.IO.Pipes;

namespace Launcher.Core.LifeCycle.InterProcess;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class PrivateNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", PrivateNamedPipe.PrivateName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);

    public bool TryRedirectActivationTo(AppActivationArguments args)
    {
        if (!clientStream.TryConnectOnce())
        {
            return false;
        }

        try
        {
            clientStream.WritePacket(PrivateNamedPipe.PrivateVersion, PipePacketType.Request, PipePacketCommand.RequestElevationStatus);
            clientStream.ReadPacket(out PipePacketHeader _, out ElevationStatusResponse? response);
            ArgumentNullException.ThrowIfNull(response);

            // Prefer elevated instance
            if (LauncherRuntime.IsProcessElevated && !response.IsElevated)
            {
                // Notify previous instance to exit
                clientStream.WritePacket(PrivateNamedPipe.PrivateVersion, PipePacketType.SessionTermination, PipePacketCommand.Exit);
                clientStream.Flush();
                WaitPreviousProcessExit(response);

                // Retain the elevated instance
                return false;
            }

            // Redirect to previous instance
            LauncherActivationArguments LauncherArgs = LauncherActivationArguments.FromAppActivationArguments(args, isRedirected: true);
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.PrivateVersion, PipePacketType.Request, PipePacketCommand.RedirectActivation, LauncherArgs);
            clientStream.WritePacket(PrivateNamedPipe.PrivateVersion, PipePacketType.SessionTermination, PipePacketCommand.None);
            clientStream.Flush();

            return true;
        }
        catch (IOException)
        {
            // Pipe is broken.
            return false;
        }
    }

    public void Dispose()
    {
        clientStream.Dispose();
    }

    private static void WaitPreviousProcessExit(ElevationStatusResponse response)
    {
        if (!ProcessFactory.TryGetById(response.ProcessId, out IProcess? process))
        {
            return;
        }

        if (process is { HasExited: false })
        {
            process.SafeWaitForExit();
        }

        SpinWaitPolyfill.SpinUntil(response, static response => !ProcessFactory.TryGetById(response.ProcessId, out _));
    }
}
