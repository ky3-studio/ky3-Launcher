using BenchmarkDotNet.Running;
using ky3launcher.Benchmarks;

BenchmarkSwitcher.FromAssembly(typeof(SpanReaderBenchmarks).Assembly).Run(args);
Console.WriteLine("Hello, World!");
