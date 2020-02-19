using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EfEagerLoad.Benchmarks.Data;
using EfEagerLoad.Benchmarks.Model;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Benchmarks.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class IncludeFinder
    {
        //private const int OperationsPerInvoke = 100;

        private TestDbContext _testDbContext;
        private EagerLoadContext _context;
        private EagerLoadAttributeIncludeStrategy _strategy;
        private Engine.IncludeFinder _includeFinder;

        [Benchmark(Baseline = true)]
        public IList<string> Recurse_Baseline()
        {
            _context.IncludePathsToInclude.Clear();
            return _includeFinder.BuildIncludePathsForRootType(_context);
        }

        [Benchmark]
        public IList<string> Iterate()
        {
            _context.IncludePathsToInclude.Clear();
            return _includeFinder.BuildIncludePathsForRootType2(_context);
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _testDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

            _strategy = new EagerLoadAttributeIncludeStrategy();

            _context = new EagerLoadContext(_testDbContext, _strategy, new string[0], IncludeExecution.NoCache, typeof(Book));

            _includeFinder = new Engine.IncludeFinder();
        }
    }
}
