//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Win32.Foundation;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace kyxsan.Core.IO.HPatch;

internal static unsafe class StreamIO
{
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL FileSegmentRead(void* input, ulong position, byte* start, byte* end)
    {
        return ((StreamInput*)input)->Handle<FileSegment>().Target is { } file && file.Read(position, start, end);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL StreamRead(void* input, ulong position, byte* start, byte* end)
    {
        if (((StreamInput*)input)->Handle<Stream>().Target is not { } stream)
        {
            return false;
        }

        try
        {
            stream.Position = (long)position;
            stream.ReadExactly(new(start, (int)(end - start)));
            return true;
        }
        catch
        {
            return false;
        }
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL FileSegmentWrite(void* output, ulong position, byte* start, byte* end)
    {
        return ((StreamOutput*)output)->Handle<FileSegment>().Target is { } file && file.Write(position, start, end);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvCdecl)])]
    public static BOOL StreamWrite(void* output, ulong position, byte* start, byte* end)
    {
        if (((StreamOutput*)output)->Handle<Stream>().Target is not { } stream)
        {
            return false;
        }

        try
        {
            stream.Position = (long)position;
            stream.Write(new(start, (int)(end - start)));
            return true;
        }
        catch
        {
            return false;
        }
    }
}