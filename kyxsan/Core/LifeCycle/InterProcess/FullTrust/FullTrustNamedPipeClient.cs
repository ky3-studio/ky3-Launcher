//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using System.IO.Pipes;

namespace kyxsan.Core.LifeCycle.InterProcess.FullTrust;

internal sealed partial class FullTrustNamedPipeClient : IDisposable
{
    private readonly NamedPipeClientStream clientStream = new(".", PrivateNamedPipe.FullTrustName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
    private readonly Lock syncRoot = new();

    public void Dispose()
    {
        lock (syncRoot)
        {
            clientStream.Dispose();
        }
    }

    public void Create(FullTrustProcessStartInfoRequest request)
    {
        lock (syncRoot)
        {
            EnsureConnected();
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.FullTrustVersion, FullTrustPipePacketType.Request, FullTrustPipePacketCommand.Create, request);
            clientStream.ReadPacket(out FullTrustPipePacketHeader header);
            kyxsanException.ThrowIf(header is not { Type: FullTrustPipePacketType.Response, Command: FullTrustPipePacketCommand.Create }, "Unexpected pipe result");
        }
    }

    public uint StartProcess()
    {
        lock (syncRoot)
        {
            EnsureConnected();
            clientStream.WritePacket(PrivateNamedPipe.FullTrustVersion, FullTrustPipePacketType.Request, FullTrustPipePacketCommand.StartProcess);
            clientStream.ReadPacket(out FullTrustPipePacketHeader header, out FullTrustStartProcessResult? result);
            kyxsanException.ThrowIf(header is not { Type: FullTrustPipePacketType.Response, Command: FullTrustPipePacketCommand.StartProcess }, "Unexpected pipe result");

            if (result is null || !result.Succeeded)
            {
                throw kyxsanException.Throw($"Failed to start full trust process: [{result?.ErrorMessage}]");
            }

            return result.ProcessId;
        }
    }

    public void LoadLibrary(FullTrustLoadLibraryRequest request)
    {
        lock (syncRoot)
        {
            EnsureConnected();
            clientStream.WritePacketWithJsonContent(PrivateNamedPipe.FullTrustVersion, FullTrustPipePacketType.Request, FullTrustPipePacketCommand.LoadLibrary, request);
            clientStream.ReadPacket(out FullTrustPipePacketHeader header, out FullTrustLoadLibraryResult? result);
            kyxsanException.ThrowIf(header is not { Type: FullTrustPipePacketType.Response, Command: FullTrustPipePacketCommand.LoadLibrary }, "Unexpected pipe result");

            if (result is null || !result.Succeeded)
            {
                throw kyxsanException.Throw($"Failed to load library on full trust process: [{result?.ErrorMessage}]");
            }
        }
    }

    public void ResumeMainThread()
    {
        lock (syncRoot)
        {
            EnsureConnected();
            clientStream.WritePacket(PrivateNamedPipe.FullTrustVersion, FullTrustPipePacketType.Request, FullTrustPipePacketCommand.ResumeMainThread);
            clientStream.ReadPacket(out FullTrustPipePacketHeader header, out FullTrustResumeMainThreadResult? result);
            kyxsanException.ThrowIf(header is not { Type: FullTrustPipePacketType.Response, Command: FullTrustPipePacketCommand.ResumeMainThread }, "Unexpected pipe result");

            if (result is null || !result.Succeeded)
            {
                throw kyxsanException.Throw($"Failed to resume main thread: [{result?.ErrorMessage}]");
            }

            clientStream.WritePacket(PrivateNamedPipe.FullTrustVersion, FullTrustPipePacketType.SessionTermination, FullTrustPipePacketCommand.None);
        }
    }

    private void EnsureConnected()
    {
        if (!clientStream.IsConnected)
        {
            clientStream.Connect();
        }
    }
}