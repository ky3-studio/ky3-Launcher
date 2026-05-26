//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Runtime.InteropServices;
using System.Text;
using static Snap.ZStandard.Zstandard;

namespace kyxsan.Core.IO.Compression.Zstandard;

// See https://github.com/skbkontur/ZstdNet
internal sealed class ZstandardException : Exception
{
    public ZstandardException(string message)
        : base(message)
    {
    }

    public unsafe ZstandardException(nuint code)
        : base(Encoding.UTF8.GetString(MemoryMarshal.CreateReadOnlySpanFromNullTerminated((byte*)ZSTD_getErrorName(code))))
    {
        ErrorCode = code;
    }

    public nuint ErrorCode { get; set; }

    public static unsafe void ThrowIfNull(void* ptr, string message)
    {
        if (ptr is null)
        {
            throw new ZstandardException(message);
        }
    }

    public static void ThrowIfError(nuint code)
    {
        if (ZSTD_isError(code) is not 0)
        {
            throw new ZstandardException(code);
        }
    }
}