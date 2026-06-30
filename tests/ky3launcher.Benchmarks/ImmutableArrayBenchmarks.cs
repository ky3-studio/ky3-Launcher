using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using Launcher.Extension;

namespace ky3launcher.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class ImmutableArrayBenchmarks
{
    private ImmutableArray<int> source;
    private ImmutableArray<int> defaultArray;

    [GlobalSetup]
    public void Setup()
    {
        source = [.. Enumerable.Range(0, 1000)];
        defaultArray = default;
    }

    [Benchmark]
    public ImmutableArray<int> EmptyIfDefault_NonDefault()
    {
        return source.EmptyIfDefault();
    }

    [Benchmark]
    public ImmutableArray<int> EmptyIfDefault_Default()
    {
        return defaultArray.EmptyIfDefault();
    }

    [Benchmark]
    public ImmutableArray<int> SelectAsArray()
    {
        return source.SelectAsArray(static x => x * 2);
    }

    [Benchmark]
    public ImmutableArray<int> Reverse()
    {
        return source.Reverse();
    }
}
