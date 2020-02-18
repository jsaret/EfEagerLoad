using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfEagerLoad.Common;
using EfEagerLoad.ConsoleTester.Configuration;
using EfEagerLoad.ConsoleTester.Data;
using EfEagerLoad.ConsoleTester.Extensions;
using EfEagerLoad.ConsoleTester.Model;
using EfEagerLoad.Engine;
using EfEagerLoad.Extensions;
using EfEagerLoad.IncludeStrategies;
using JetBrains.Profiler.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfEagerLoad.ConsoleTester
{
    public class Program
    {
        private const string ConnectionString = "Server=.;Database=EfEagerLoad;Trusted_Connection=True;";
        private static readonly TestDbContext _testDbContext = new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                                                    .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        public static async Task Main(string[] args)
        {
            var value = Perf();

            if (value == null)
            {
                Console.WriteLine();
            }


            //Func<Task> runFunc = Run;
            //await runFunc.RunInConsole();
        }

        private static readonly IQueryable<Book> BookQuery = new Book[0].AsQueryable();

        public static object Perf()
        {
            EagerLoadContext.SkipEntityFrameworkCheckForTesting = true;
            EagerLoadContext.SkipQueryIncludeForTesting = true;

            var item = BookQuery.EagerLoad(_testDbContext, IncludeExecution.NoCache);
            Enumerable.Range(0, 2).ForEach(_ =>
            {
                item = BookQuery.EagerLoad(_testDbContext, IncludeExecution.NoCache);
            });


            GC.Collect(0);
            Thread.Sleep(1);

            MemoryProfiler.CollectAllocations(true);

            MemoryProfiler.GetSnapshot();
            for (var i = 0; i < 5; i++)
            {
                item = BookQuery.EagerLoad(_testDbContext, IncludeExecution.NoCache);
                MemoryProfiler.GetSnapshot();
            }

            MemoryProfiler.CollectAllocations(false);

            return item;
        }

        public static async Task Run()
        {
            var serviceProvider = await SetupServices();
            var testRunner = serviceProvider.GetRequiredService<TestRunner>();

            EagerLoadContext.InitializeServiceProvider(serviceProvider);
            await testRunner.RunTest1();
            await testRunner.RunTest2();
            //await testRunner.RunTest3();
            //await testRunner.RunTest4();

            //await testRunner.RunTest5();
            //await testRunner.RunTest6();
        }

        private static async Task<IServiceProvider> SetupServices()
        {
            Console.WriteLine("________________ Setting up Services ________________");
            var services = new ServiceCollection();
            services.ConfigureServices();
            services.ConfigureEntityFramework<TestDbContext>(ConnectionString);
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.SetupDatabase<TestDbContext>(shouldRun: false, recreateDataBase: false);
            await serviceProvider.SetupDataInDatabase(shouldRun: false);
            Console.WriteLine("________________ Services Setup ________________");
            Console.WriteLine();
            return serviceProvider;
        }
    }
}
