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

internal static partial class MemoryPoolExtension
{
    extension<T>(MemoryPool<T> memoryPool)
    {
        public IMemoryOwner<T> RentExactly(int bufferSize)
        {
            IMemoryOwner<T> memoryOwner = memoryPool.Rent(bufferSize);
            return new ExactSizedMemoryOwner<T>(memoryOwner, bufferSize);
        }
    }

    private sealed partial class ExactSizedMemoryOwner<T> : IMemoryOwner<T>
    {
        private readonly IMemoryOwner<T> owner;
        private readonly int bufferSize;

        public ExactSizedMemoryOwner(IMemoryOwner<T> owner, int bufferSize)
        {
            this.owner = owner;
            this.bufferSize = bufferSize;
        }

        public Memory<T> Memory { get => owner.Memory[..bufferSize]; }

        public void Dispose()
        {
            owner.Dispose();
        }
    }
}