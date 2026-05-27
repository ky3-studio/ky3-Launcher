//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.
using Google.Protobuf.Collections;
using System.Runtime.CompilerServices;

namespace kyxsan.Core.Protobuf.Collection;

internal static class ProtobufCollectionsMarshal
{
    public static Span<T> AsSpan<T>(RepeatedField<T> field)
    {
        return new(PrivateGetArray(field), 0, field.Count);
    }

    // private T[] array
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "array")]
    private static extern ref T[] PrivateGetArray<T>(RepeatedField<T> field);
}