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
using kyxsan.Win32.Foundation;

namespace kyxsan.Factory.Process;

internal sealed partial class DiagnosticsProcess : IProcess
{
    private readonly System.Diagnostics.Process process;

    public DiagnosticsProcess(System.Diagnostics.Process process)
    {
        this.process = process;
    }

    public int Id { get => process.Id; }

    public nint Handle { get => process.Handle; }

    public HWND MainWindowHandle { get => process.MainWindowHandle; }

    public bool HasExited
    {
        get
        {
            try
            {
                return process.HasExited;
            }
            catch (InvalidOperationException)
            {
                // No process is associated with this object.
                return true;
            }
            catch (Win32Exception win32Ex)
            {
                if ((WIN32_ERROR)win32Ex.NativeErrorCode == WIN32_ERROR.ERROR_ACCESS_DENIED)
                {
                    return false;
                }

                throw;
            }
        }
    }

    public int ExitCode { get => process.ExitCode; }

    public void Start()
    {
        process.Start();
    }

    public void ResumeMainThread()
    {
        kyxsanException.NotSupported("ResumeMainThread is not supported for System.Diagnostics.Process.");
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
        process.Dispose();
    }
}