using System;
using EfEagerLoad.Tests.Testing.Model;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Tests.Testing
{
    public class TestDbContext : DbContext
    {
        public static readonly TestDbContext Instance = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>().Options);
            //.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }


        public DbSet<Country> Countries { get; set; }

        public DbSet<Publisher> Publishers { get; set; }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Category> Category { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
