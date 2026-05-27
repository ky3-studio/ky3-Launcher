//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

namespace kyxsan.Core.Threading;

internal sealed partial class ExclusiveTokenProvider : IDisposable
{
    private readonly Lock syncRoot = new();
    private volatile CancellationTokenSource? cts = new();

    public CancellationToken CurrentToken
    {
        get
        {
            lock (syncRoot)
            {
                return cts?.Token ?? new(true);
            }
        }
    }

    public CancellationToken GetNewToken()
    {
        lock (syncRoot)
        {
            if (cts is not null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = new();
            return cts.Token;
        }
    }

    public void Cancel()
    {
        lock (syncRoot)
        {
            cts?.Cancel();
        }
    }

    public void Dispose()
    {
        lock (syncRoot)
        {
            if (cts is not null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = default!;
        }
    }
}