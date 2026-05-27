//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using Google.Protobuf;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.Protobuf;

internal static class ByteStringMarshal
{
    /// <summary>
    /// Creates a new ByteString from the given memory. The memory is <b>not</b>
    /// copied, and must not be modified after this method is called.
    /// </summary>
    /// <param name="bytes">source bytes</param>
    /// <returns>A new ByteString instance</returns>
    public static ByteString Create(ReadOnlyMemory<byte> bytes)
    {
        return CreateByteString(bytes);
    }

    // private ByteString(ReadOnlyMemory<byte> bytes)
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    private static extern ByteString CreateByteString(ReadOnlyMemory<byte> bytes);
}