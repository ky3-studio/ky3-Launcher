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
using kyxsan.Core.LifeCycle.InterProcess.FullTrust;
using kyxsan.Win32.Foundation;

namespace kyxsan.Factory.Process;

internal sealed partial class FullTrustProcess : IProcess
{
    private readonly FullTrustNamedPipeClient client = new();
    private global::System.Diagnostics.Process? process;

    public FullTrustProcess(FullTrustProcessStartInfoRequest startInfo)
    {
        client.Create(startInfo);
    }

    public int Id { get => process?.Id ?? throw kyxsanException.InvalidOperation("Process not created"); }

    public nint Handle { get => process?.Handle ?? throw kyxsanException.InvalidOperation("Process not created"); }

    public HWND MainWindowHandle { get => process?.MainWindowHandle ?? throw kyxsanException.InvalidOperation("Process not created"); }

    public bool HasExited { get => process?.HasExited ?? throw kyxsanException.InvalidOperation("Process not created"); }

    public int ExitCode { get => process?.ExitCode ?? throw kyxsanException.InvalidOperation("Process not created"); }

    public void Dispose()
    {
        client.Dispose();
        process?.Dispose();
    }

    public void Kill()
    {
        if (process is null)
        {
            throw kyxsanException.InvalidOperation("Process not created");
        }

        process.Kill();
    }

    public void ResumeMainThread()
    {
        client.ResumeMainThread();
    }

    public void Start()
    {
        uint processId = client.StartProcess();
        process = global::System.Diagnostics.Process.GetProcessById((int)processId);
    }

    public void WaitForExit()
    {
        if (process is null)
        {
            throw kyxsanException.InvalidOperation("Process not created");
        }

        process.WaitForExit();
    }

    internal void LoadLibrary(FullTrustLoadLibraryRequest request)
    {
        client.LoadLibrary(request);
    }
}