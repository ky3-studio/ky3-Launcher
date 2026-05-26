//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Diagnostics;
using kyxsan.Win32;
using kyxsan.Win32.Foundation;

namespace kyxsan.Factory.Process;

internal sealed partial class NativeProcess : IProcess
{
    private readonly kyxsanNativeProcess process;

    public NativeProcess(kyxsanNativeProcess process)
    {
        this.process = process;
    }

    public int Id { get => (int)process.Id; }

    public nint Handle { get => process.ProcessHandle; }

    public HWND MainWindowHandle { get => process.MainWindowHandle; }

    public bool HasExited { get => process.GetExitCodeProcess(out _); }

    public int ExitCode
    {
        get
        {
            process.GetExitCodeProcess(out uint code);
            return (int)code;
        }
    }

    public void Start()
    {
        process.Start();
    }

    public void ResumeMainThread()
    {
        process.ResumeMainThread();
    }

    public void WaitForExit()
    {
        process.WaitForExit();
    }

    public void Kill()
    {
        process.Kill();
    }

    public void Dispose()
    {
    }
}