//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using System.Buffers;

namespace kyxsan.Extension;

internal static partial class MemoryOwnerExtension
{
    extension<T>(IMemoryOwner<T>)
    {
        public static IMemoryOwner<T> Empty
        {
            get => EmptyMemoryOwner<T>.Instance;
        }
    }

    private sealed partial class EmptyMemoryOwner<T> : IMemoryOwner<T>
    {
        public static readonly EmptyMemoryOwner<T> Instance = new();

        public Memory<T> Memory { get => Memory<T>.Empty; }

        public void Dispose()
        {
            // Do nothing, so that can be disposed multiple times safely.
        }
    }
}