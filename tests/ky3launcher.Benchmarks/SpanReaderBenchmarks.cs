using BenchmarkDotNet.Attributes;
using Launcher.Core;

namespace ky3launcher.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class SpanReaderBenchmarks
{
    private byte[] data = null!;

    [GlobalSetup]
    public void Setup()
    {
        data = new byte[4096];
        Random.Shared.NextBytes(data);
        data[512] = 0xFF;
        data[1024] = 0xFE;
        data[2048] = 0xFD;
    }

    [Benchmark]
    public int TryReadTo_SingleDelimiter()
    {
        SpanReader<byte> reader = new(data);
        int found = 0;
        while (reader.TryReadTo(0xFF, out _))
        {
            found++;
        }

        return found;
    }

    [Benchmark]
    public int TryRead_Sequential()
    {
        SpanReader<byte> reader = new(data);
        int count = 0;
        while (reader.TryRead(out byte _))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int TryRead_Chunks()
    {
        SpanReader<byte> reader = new(data);
        int count = 0;
        while (reader.TryRead(64, out _))
        {
            count++;
        }

        return count;
    }

    [Benchmark]
    public int AdvancePast_Zeros()
    {
        byte[] zeroed = new byte[1024];
        SpanReader<byte> reader = new(zeroed);
        return reader.AdvancePast(0);
    }

    [Benchmark]
    public void Rewind_And_Reset()
    {
        SpanReader<byte> reader = new(data);
        reader.Advance(2048);
        reader.Rewind(1024);
        reader.Reset();
    }
}
