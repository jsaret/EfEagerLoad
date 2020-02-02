using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using EfEagerLoad.ConsoleTester.Data;
using EfEagerLoad.ConsoleTester.Configuration;
using EfEagerLoad.ConsoleTester.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace EfEagerLoad.ConsoleTester
{
    public class Program
    {
        private const string ConnectionString = "Server=.;Database=EfEagerLoad;Trusted_Connection=True;";

        public static async Task Main(string[] args)
        {
            Func<Task> runFunc = Run;
            await runFunc.RunInConsole();
        }

        public static async Task Run()
        {
            var serviceProvider = await SetupServices();
            var testRunner = serviceProvider.GetRequiredService<TestRunner>();
            //var summary = BenchmarkRunner.Run<Benchmarks>();
            await testRunner.RunTest1();
        }

        public static async Task<IServiceProvider> SetupServices()
        {
            Console.WriteLine("________________ Setting up Services ________________");
            var services = new ServiceCollection();
            services.ConfigureServices();
            services.ConfigureEntityFramework<TestDbContext>(ConnectionString);
            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.SetupDatabase<TestDbContext>(recreateDataBase: false);
            //await serviceProvider.SetupDataInDatabase();
            Console.WriteLine("________________ Services Setup ________________");
            Console.WriteLine();
            return serviceProvider;
        }
    }
}
