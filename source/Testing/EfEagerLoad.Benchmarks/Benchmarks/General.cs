using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EfEagerLoad.Benchmarks.Data;
using EfEagerLoad.Benchmarks.Model;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Benchmarks.Benchmarks
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class General
    {
        private static readonly IQueryable<Book> BookQuery = new Book[0].AsQueryable();

        private TestDbContext _testDbContext;

        [GlobalSetup]
        public void GlobalCleanup()
        {
            EagerLoadContext.SkipEntityFrameworkCheckForTesting = true;
            _testDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        }

        [Benchmark(Baseline = true)]
        public IList<Book> HandCoded_Navigation()
        {
            return BookQuery.Include(nameof(Book.Author))
                .Include($"{nameof(Book.Author)}.{ nameof(Author.Books)}")
                .Include(nameof(Book.Category))
                .Include(nameof(Book.Publisher)).ToArray();
        }

        [Benchmark]
        public IList<Book> HandCoded_Expressions()
        {
            return BookQuery.Include(book => book.Author).ThenInclude(author => author.Books)
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
            return BookQuery.EagerLoad(_testDbContext, $"{nameof(Book.Author)}.{ nameof(Author.Books)}").ToArray();
        }

        [Benchmark]
        public IList<Book> EfEagerLoad_NotCached()
        {
            return BookQuery.EagerLoad(_testDbContext, IncludeExecution.NoCache).ToArray();
        }

        [Benchmark]
        public IList<Book> EfEagerLoad_NotCached_IgnoringIncludePaths()
        {
            return BookQuery.EagerLoad(_testDbContext, IncludeExecution.NoCache, 
                $"{nameof(Book.Author)}.{ nameof(Author.Books)}").ToArray();
        }

        [Benchmark]
        public IList<Book> EfEagerLoad_Cached()
        {
            return BookQuery.EagerLoad(_testDbContext, true).ToArray();
        }
        
    }
}
