//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Diagnostics;

internal static class ProcessExtension
{
    extension(IProcess process)
    {
        public bool IsRunning
        {
            get
            {
                try
                {
                    return !process.HasExited;
                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                    return false;
                }
            }
        }

        public void SafeWaitForExit()
        {
            try
            {
                process.WaitForExit();
            }
            catch (Exception e)
            {
                SentrySdk.CaptureException(e);
            }
        }
    }
}