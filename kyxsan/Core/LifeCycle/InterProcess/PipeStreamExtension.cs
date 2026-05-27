//  _  ____   ____  ______    _    _   _          ____  _   _    _    ____  _   _ _   _ _____  _    ___
// | |/ /\ \ / /\ \/ / ___|  / \  | \ | | __  __ / ___|| \ | |  / \  |  _ \| | | | | | |_   _|/ \  / _ \
// | ' /  \ V /  \  /\___ \ / _ \ |  \| | \ \/ / \___ \|  \| | / _ \ | |_) | |_| | | | | | | / _ \| | | |
// | . \   | |   /  \ ___) / ___ \| |\  |  >  <   ___) | |\  |/ ___ \|  __/|  _  | |_| | | |/ ___ \ |_| |
// |_|\_\  |_|  /_/\_\____/_/   \_\_| \_| /_/\_\ |____/|_| \_/_/   \_\_|   |_| |_|\___/  |_/_/   \_\___/
// Copyright (c) DGP Studio. All rights reserved.
// Modified by kyxsan.
// Licensed under the MIT license.

using kyxsan.Core.ExceptionService;
using System.Buffers;
using System.IO.Hashing;
using System.IO.Pipes;

namespace kyxsan.Core.LifeCycle.InterProcess;

internal static class PipeStreamExtension
{
    extension(PipeStream stream)
    {
        public TData? ReadJsonContent<TData>(ref readonly PipePacketHeader header)
        {
            using (IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.RentExactly(header.ContentLength))
            {
                Span<byte> content = memoryOwner.Memory.Span;
                stream.ReadExactly(content);
                kyxsanException.ThrowIf(XxHash64.HashToUInt64(content) != header.Checksum, "PipePacket Content Hash incorrect");

                return JsonSerializer.Deserialize<TData>(content);
            }
        }

        public void ReadPacket<TData>(out PipePacketHeader header, out TData? data)
            where TData : class
        {
            data = default;

            stream.ReadPacket(out header);
            if (header.ContentType is PipePacketContentType.Json)
            {
                data = stream.ReadJsonContent<TData>(in header);
            }
        }

        public unsafe void ReadPacket(out PipePacketHeader header)
        {
            fixed (PipePacketHeader* pHeader = &header)
            {
                stream.ReadExactly(new(pHeader, sizeof(PipePacketHeader)));
            }
        }

        public void WritePacketWithJsonContent<TData>(byte version, PipePacketType type, PipePacketCommand command, TData data)
        {
            PipePacketHeader header = default;
            header.Version = version;
            header.Type = type;
            header.Command = command;
            header.ContentType = PipePacketContentType.Json;

            stream.WritePacket(ref header, JsonSerializer.SerializeToUtf8Bytes(data));
        }

        public void WritePacket(ref PipePacketHeader header, ReadOnlySpan<byte> content)
        {
            header.ContentLength = content.Length;
            header.Checksum = XxHash64.HashToUInt64(content);

            stream.WritePacket(in header);
            stream.Write(content);
        }

        public void WritePacket(byte version, PipePacketType type, PipePacketCommand command)
        {
            PipePacketHeader header = default;
            header.Version = version;
            header.Type = type;
            header.Command = command;

            stream.WritePacket(in header);
        }

        public unsafe void WritePacket(ref readonly PipePacketHeader header)
        {
            fixed (PipePacketHeader* pHeader = &header)
            {
                stream.Write(new(pHeader, sizeof(PipePacketHeader)));
            }
        }
    }
}