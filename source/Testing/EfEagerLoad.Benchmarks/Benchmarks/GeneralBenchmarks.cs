using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EfEagerLoad.Benchmarks.Data;
using EfEagerLoad.Benchmarks.Model;
using EfEagerLoad.Engine;
using EfEagerLoad.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Benchmarks.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class GeneralBenchmarks
    {
        private TestDbContext _testDbContext;

        [Benchmark(Baseline = true)]
        public IList<Book> HandCoded_Navigation()
        {
            var bookQuery = new Book[0].AsQueryable();
            return bookQuery.Include(nameof(Book.Author))
                .Include($"{nameof(Book.Author)}.{ nameof(Author.Books)}")
                .Include(nameof(Book.Category))
                .Include(nameof(Book.Publisher)).ToArray();
        }

        [Benchmark]
        public IList<Book> HandCoded_Expressions()
        {
            var bookQuery = new Book[0].AsQueryable();
            return bookQuery.Include(book => book.Author).ThenInclude(author => author.Books)
                .Include(book => book.Category)
                .Include(book => book.Publisher).ThenInclude(p => p.Books)
                .ToArray();

            //return bookQuery.Include(book => book.Author).ThenInclude(author => author.Books).ThenInclude(b => b.Author)
            //    .ThenInclude(a => a.Books)
            //    .Include(book => book.Category)
            //    .Include(book => book.Publisher)
            //    .ThenInclude(p => p.Books).ThenInclude(b => b.Author).ThenInclude(a => a.Books)
            //    .ToArray();
        }

        [Benchmark]
        public IList<Book> EfEagerLoad_IgnoringIncludePaths()
        {
            var bookQuery = new Book[0].AsQueryable();
            return bookQuery.EagerLoad(_testDbContext, $"{nameof(Book.Author)}.{ nameof(Author.Books)}").ToArray();
        }

        [Benchmark]
        public IList<Book> EfEagerLoad_NotCached()
        {
            var bookQuery = new Book[0].AsQueryable();
            return bookQuery.EagerLoad(_testDbContext, IncludeExecution.NoCache).ToArray();
        }

        [Benchmark]
        public IList<Book> EfEagerLoad_Cached()
        {
            var bookQuery = new Book[0].AsQueryable();
            return bookQuery.EagerLoad(_testDbContext, true).ToArray();
        }


        [GlobalSetup]
        public void GlobalCleanup()
        {
            _testDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }
    }
}
