using System;
using System.Reflection;
using System.Threading.Tasks;
using EfEagerLoad.Testing.Data;
using EfEagerLoad.Testing.Model;
using EfEagerLoad.Testing.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfEagerLoad.ConsoleTester.Configuration
{
    public static class DatabaseConfiguration
    {
        public static void ConfigureEntityFramework<TDbContext>(this IServiceCollection services, string connectionString) where TDbContext : DbContext
        {
            services.AddDbContextPool<TDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(connectionString,
                    config =>
                    {
                        config.MigrationsAssembly(Assembly.GetAssembly(typeof(Publisher)).GetName().Name);
                    });
            });
            services.AddScoped<DbContext, TDbContext>();
            services.AddTransient<IRepository, EntityFrameworkRepository>();
        }

        public static async Task SetupDatabase<TDbContext>(this IServiceProvider serviceProvider, bool createDataBase = false, bool shouldRun = true,
                                                            bool recreateDataBase = false, bool runMigrations = false) where TDbContext : DbContext
        {
            if (!shouldRun) { return; }

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

                if (recreateDataBase)
                {
                    await dbContext.Database.EnsureDeletedAsync();
                }

                await dbContext.Database.EnsureCreatedAsync();

                if (!runMigrations && (recreateDataBase || createDataBase))
                {
                    await dbContext.Database.EnsureCreatedAsync();
                }

                if (runMigrations)
                {
                    await dbContext.Database.MigrateAsync();
                }
            }
        }

        public static async Task SetupDataInDatabase(this IServiceProvider serviceProvider, bool shouldRun = true)
        {
            if (!shouldRun) { return; }

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TestDbContext>();

                var comedyCategory = new Category { Name = "Comedy" };
                var horrorCategory = new Category { Name = "Horror" };
                var sciFiCategory = new Category { Name = "SciFi" };
                await dbContext.AddRangeAsync(comedyCategory, horrorCategory, sciFiCategory);

                var apressPublisher = new Publisher {Name = "Apress"};
                var penguinPublisher = new Publisher { Name = "Penguin" };
                var randomHousePublisher = new Publisher { Name = "Random House" };
                await dbContext.AddRangeAsync(apressPublisher, penguinPublisher, randomHousePublisher);

                var billDeveloperAuthor = new Author { Name = "Bill Developer" };
                var tolkienAuthor = new Author { Name = "Tolkien" };
                var douglasAdamsAuthor = new Author { Name = "Douglas Adams" };
                var steveKingAuthor = new Author { Name = "Bill Developer" };
                await dbContext.AddRangeAsync(billDeveloperAuthor, tolkienAuthor, douglasAdamsAuthor, steveKingAuthor);

                var book1 = new Book {Name = "Book 1", Category = comedyCategory, Author = billDeveloperAuthor, Publisher = apressPublisher};
                var book2 = new Book { Name = "Book 2", Category = horrorCategory, Author = douglasAdamsAuthor, Publisher = penguinPublisher };
                var book3 = new Book { Name = "Book 3", Category = sciFiCategory, Author = billDeveloperAuthor, Publisher = randomHousePublisher };
                var book4 = new Book { Name = "Book 4", Category = comedyCategory, Author = tolkienAuthor, Publisher = apressPublisher };
                var book5 = new Book { Name = "Book 5", Category = horrorCategory, Author = tolkienAuthor, Publisher = penguinPublisher };
                var book6 = new Book { Name = "Book 6", Category = comedyCategory, Author = douglasAdamsAuthor, Publisher = apressPublisher };
                var book7 = new Book { Name = "Book 7", Category = horrorCategory, Author = billDeveloperAuthor, Publisher = apressPublisher };
                var book8 = new Book { Name = "Book 8", Category = comedyCategory, Author = steveKingAuthor, Publisher = apressPublisher };
                var book9 = new Book { Name = "Book 9", Category = comedyCategory, Author = tolkienAuthor, Publisher = randomHousePublisher };
                var book10 = new Book { Name = "Book 10", Category = sciFiCategory, Author = steveKingAuthor, Publisher = apressPublisher };
                await dbContext.AddRangeAsync(book1, book2, book3, book4, book5, book6, book7, book8, book9, book10);


                var australiaCountry = new Country { Name = "Australia" };
                var southAfricaCountry = new Country { Name = "South Africa" };
                var unitedStatesCountry = new Country { Name = "United States of America" };

                australiaCountry.Publishers.Add(penguinPublisher);
                southAfricaCountry.Publishers.Add(apressPublisher);
                unitedStatesCountry.Publishers.Add(randomHousePublisher);
                await dbContext.AddRangeAsync(australiaCountry, southAfricaCountry, unitedStatesCountry);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
