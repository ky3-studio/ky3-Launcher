//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.CompilerServices;
using WinRT;

namespace kyxsan.Extension;

// ReSharper disable once InconsistentNaming
internal static class WinRTExtension
{
    extension(IWinRTObject? obj)
    {
        public bool IsDisposed
        {
            get => obj?.NativeObject is null || obj.NativeObject.IsDisposed;
        }
    }

    extension(IObjectReference objRef)
    {
        public bool IsDisposed
        {
            get => Volatile.Read(ref GetPrivateDisposedFlags(objRef)) is not 0;
        }
    }

    // private const int NOT_DISPOSED = 0;
    // private const int DISPOSE_PENDING = 1;
    // private const int DISPOSE_COMPLETED = 2;
    // private int _disposedFlags
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_disposedFlags")]
    private static extern ref int GetPrivateDisposedFlags(IObjectReference objRef);
}