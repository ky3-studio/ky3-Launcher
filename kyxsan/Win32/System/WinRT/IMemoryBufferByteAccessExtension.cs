//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;

namespace kyxsan.Win32.System.WinRT;

internal static class IMemoryBufferByteAccessExtension
{
    extension(IMemoryBufferByteAccess memoryBufferByteAccess)
    {
        public unsafe HRESULT GetBuffer(out byte* value, out uint capacity)
        {
            fixed (byte** value2 = &value)
            {
                fixed (uint* capacity2 = &capacity)
                {
                    return memoryBufferByteAccess.GetBuffer(value2, capacity2);
                }
            }
        }

        public unsafe HRESULT GetBuffer<T>(out Span<T> value)
            where T : unmanaged
        {
            HRESULT retVal = memoryBufferByteAccess.GetBuffer(out byte* data, out uint capacity);
            value = new(data, unchecked((int)capacity / sizeof(T)));
            return retVal;
        }
    }
}