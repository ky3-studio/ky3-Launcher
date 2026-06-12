using Google.Protobuf;
using kyxsan.Core.Protobuf;
using System.Collections.Immutable;

namespace kyxsan.Service.Yae.PlayerStore;

internal static class PlayerStoreParser
{
    public static ImmutableArray<(uint ItemId, uint Count)> Parse(ByteString bytes)
    {
        ImmutableArray<(uint, uint)>.Builder results = ImmutableArray.CreateBuilder<(uint, uint)>();

        using (CodedInputStream stream = bytes.CreateCodedInput())
        {
            try
            {
                while (stream.TryReadTag(out uint tag))
                {
                    switch (WireFormat.GetTagWireType(tag))
                    {
                        case WireFormat.WireType.Varint:
                            _ = stream.ReadUInt64();
                            continue;

                        case WireFormat.WireType.LengthDelimited:
                            // Each top-level LD field = one Item's raw field bytes
                            using (CodedInputStream itemStream = stream.UnsafeReadLengthDelimitedStream())
                            {
                                (uint itemId, uint count) = ParseItemFields(itemStream);
                                if (itemId > 0 && count > 0)
                                {
                                    results.Add((itemId, count));
                                }
                            }

                            break;

                        default:
                            SkipField(stream, WireFormat.GetTagWireType(tag));
                            break;
                    }
                }
            }
            catch (InvalidProtocolBufferException)
            {
            }
        }

        return results.ToImmutable();
    }

    private static (uint ItemId, uint Count) ParseItemFields(CodedInputStream stream)
    {
        uint itemId = 0;
        uint materialCount = 0;

        try
        {
            while (stream.TryReadTag(out uint tag))
            {
                int fieldNumber = WireFormat.GetTagFieldNumber(tag);
                WireFormat.WireType wireType = WireFormat.GetTagWireType(tag);

                switch (fieldNumber)
                {
                    case 1 when wireType is WireFormat.WireType.Varint:
                        itemId = stream.ReadUInt32();
                        break;

                    case 2 when wireType is WireFormat.WireType.Varint:
                        _ = stream.ReadUInt64();
                        break;

                    // Material detail (field 5): count is sub-field 1
                    case 5 when wireType is WireFormat.WireType.LengthDelimited:
                        materialCount = ParseCountField(stream);
                        break;

                    // Furniture detail (field 7): count is also sub-field 1
                    case 7 when wireType is WireFormat.WireType.LengthDelimited:
                        materialCount = ParseCountField(stream);
                        break;

                    default:
                        SkipField(stream, wireType);
                        break;
                }
            }
        }
        catch (InvalidProtocolBufferException)
        {
        }

        return (itemId, materialCount);
    }

    private static uint ParseCountField(CodedInputStream outerStream)
    {
        uint count = 0;
        using (CodedInputStream stream = outerStream.UnsafeReadLengthDelimitedStream())
        {
            try
            {
                while (stream.TryReadTag(out uint tag))
                {
                    int fieldNumber = WireFormat.GetTagFieldNumber(tag);
                    WireFormat.WireType wireType = WireFormat.GetTagWireType(tag);

                    if (fieldNumber == 1 && wireType is WireFormat.WireType.Varint)
                    {
                        count = stream.ReadUInt32();
                    }
                    else
                    {
                        SkipField(stream, wireType);
                    }
                }
            }
            catch (InvalidProtocolBufferException)
            {
            }
        }

        return count;
    }

    private static void SkipField(CodedInputStream stream, WireFormat.WireType wireType)
    {
        switch (wireType)
        {
            case WireFormat.WireType.Varint:
                _ = stream.ReadUInt64();
                break;
            case WireFormat.WireType.Fixed64:
                _ = stream.ReadFixed64();
                break;
            case WireFormat.WireType.LengthDelimited:
                _ = stream.UnsafeReadLengthDelimitedStream();
                break;
            case WireFormat.WireType.Fixed32:
                _ = stream.ReadFixed32();
                break;
        }
    }
}
