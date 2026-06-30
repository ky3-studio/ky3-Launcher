using BenchmarkDotNet.Attributes;
using Launcher.Extension;

namespace ky3launcher.Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
public class StringExtensionBenchmarks
{
    private string testString = null!;
    private string[] candidates = null!;

    [GlobalSetup]
    public void Setup()
    {
        testString = "https://api.example.com/v2/users/12345/profile?lang=zh-CN&format=json";
        candidates = ["https://api.example.com", "https://cdn.example.net", "https://api.other.io"];
    }

    [Benchmark]
    public bool EqualsAny_Found()
    {
        return testString.EqualsAny(candidates, StringComparison.Ordinal);
    }

    [Benchmark]
    public bool EqualsAny_NotFound()
    {
        return "nothing".EqualsAny(candidates, StringComparison.Ordinal);
    }

    [Benchmark]
    public string TrimEnd_Chars()
    {
        return "hello world!!!".TrimEnd("!");
    }

    [Benchmark]
    public Uri? ToUri()
    {
        return testString.ToUri();
    }
}
