using System;
using EfEagerLoad.Testing.Model;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Testing.Data
{
    public class TestDbContext : DbContext
    {

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
