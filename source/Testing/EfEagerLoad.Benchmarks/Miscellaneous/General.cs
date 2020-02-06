using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using EfEagerLoad.Extensions;
using EfEagerLoad.IncludeStrategy;
using EfEagerLoad.Testing.Data;
using EfEagerLoad.Testing.Model;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Benchmarks.Miscellaneous
{
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    public class General
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
                .Include(book => book.Publisher).ToArray();
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
