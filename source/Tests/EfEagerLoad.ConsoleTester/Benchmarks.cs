using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EfEagerLoad.ConsoleTester.Data;
using EfEagerLoad.ConsoleTester.Model;
using EfEagerLoad.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfEagerLoad.ConsoleTester
{
    public class Benchmarks
    {
        private TestDbContext _testDbContext;

        [Benchmark(Baseline = true)]
        public void OldCode()
        {
            var bookQuery = new Book[0].AsQueryable();
            bookQuery = bookQuery.EagerLoad(_testDbContext, true, "Test");
        }

        [Benchmark]
        public void NewCode()
        {
            var bookQuery = new Book[0].AsQueryable();
            bookQuery = bookQuery.EagerLoad(_testDbContext, true);
        }

        [GlobalSetup]
        public void GlobalCleanup()
        {
            _testDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}
