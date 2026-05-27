//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using System.IO;

namespace kyxsan.Core.IO.HPatch;

internal sealed unsafe partial class StreamInputStream : Stream
{
    private readonly StreamInput* input;
    private readonly ulong begin;
    private readonly ulong end;
    private ulong position;

    public StreamInputStream(StreamInput* input, ulong begin, ulong end)
    {
        this.input = input;
        this.begin = begin;
        this.end = end;
        position = begin;
    }

    public override bool CanRead { get => true; }

    public override bool CanSeek { get => true; }

    public override bool CanWrite { get => false; }

    public override long Length { get => (long)(end - begin); }

    public override long Position { get => (long)(position - begin); set => position = (ulong)value + begin; }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (count > (int)(end - position))
        {
            count = (int)(end - position);
        }

        if (count <= 0)
        {
            return 0;
        }

        fixed (byte* pBuffer = buffer)
        {
            if (input->Read(input, position, pBuffer, pBuffer + count))
            {
                position += (ulong)count;
                return count;
            }
        }

        return 0;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw kyxsanException.NotSupported();
    }

    public override void SetLength(long value)
    {
        throw kyxsanException.NotSupported();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw kyxsanException.NotSupported();
    }
}