//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.Win32;

internal static unsafe class MemoryUtilities
{
    public static void Patch(ReadOnlySpan<char> moduleName, uint offset, int size, Action<Span<byte>> action)
    {
        fixed (char* pModuleName = moduleName)
        {
            using (GCHandle<Action<Span<byte>>> actionHandle = new(action))
            {
                MemoryUtilitiesPatch(pModuleName, offset, size, PatchCallback.Create(&MemoryUtilitiesPatchCallback), actionHandle);
            }
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static HRESULT MemoryUtilitiesPatchCallback(byte* ptr, int size, GCHandle<Action<Span<byte>>> state)
    {
        try
        {
            Action<Span<byte>>? action = state.Target;
            Span<byte> span = new(ptr, size);
            action?.Invoke(span);
            return 0; // S_OK
        }
        catch
        {
            return HRESULT.E_UNEXPECTED;
        }
    }

    [DllImport(kyxsanNativeMethods.DllName, ExactSpelling = true)]
    private static extern HRESULT MemoryUtilitiesPatch(PCWSTR moduleName, uint offset, int size, PatchCallback callback, GCHandle<Action<Span<byte>>> state);

    private readonly struct PatchCallback
    {
        private readonly delegate* unmanaged[Stdcall]<byte*, int, GCHandle<Action<Span<byte>>>, HRESULT> value;

        private PatchCallback(delegate* unmanaged[Stdcall]<byte*, int, GCHandle<Action<Span<byte>>>, HRESULT>  value)
        {
            this.value = value;
        }

        public static PatchCallback Create(delegate* unmanaged[Stdcall]<byte*, int, GCHandle<Action<Span<byte>>>, HRESULT> value)
        {
            return new(value);
        }
    }
}