using System;
using BenchmarkDotNet.Running;
using EfEagerLoad.Benchmarks.Benchmarks;

namespace EfEagerLoad.Benchmarks
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<GeneralBenchmarks>();
        }
    }
}
