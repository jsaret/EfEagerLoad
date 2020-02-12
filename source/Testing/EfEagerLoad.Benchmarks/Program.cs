using System;
using BenchmarkDotNet.Running;
using EfEagerLoad.Benchmarks.Benchmarks;

namespace EfEagerLoad.Benchmarks
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}
