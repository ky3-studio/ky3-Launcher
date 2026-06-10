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

internal static class CodedInputStreamMarshal
{
    extension(CodedInputStream stream)
    {
        public bool TryPeekTag(out uint tag)
        {
            tag = stream.PeekTag();
            return tag is not 0;
        }

        public bool TryReadTag(out uint tag)
        {
            tag = stream.ReadTag();
            return tag is not 0;
        }

        public CodedInputStream UnsafeReadLengthDelimitedStream()
        {
            return new(ReadRawBytes(stream, stream.ReadLength()));
        }
    }

    // internal byte[] ReadRawBytes(int size)
    [UnsafeAccessor(UnsafeAccessorKind.Method)]
    private static extern byte[] ReadRawBytes(CodedInputStream stream, int size);
}