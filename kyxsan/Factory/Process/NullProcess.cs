//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.Diagnostics;
using kyxsan.Win32.Foundation;

namespace kyxsan.Factory.Process;

internal sealed class NullProcess : IProcess
{
    public int Id => 0;

    public nint Handle => 0;

    public HWND MainWindowHandle => default;

    public bool HasExited => true;

    public int ExitCode => 0;

    public void Start()
    {
    }

    public void ResumeMainThread()
    {
    }

    public void WaitForExit()
    {
    }

    public void Kill()
    {
    }

    public void Dispose()
    {
    }
}