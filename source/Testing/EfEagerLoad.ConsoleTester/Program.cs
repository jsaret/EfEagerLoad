using System;
using System.Threading.Tasks;
using EfEagerLoad.ConsoleTester.Configuration;
using EfEagerLoad.Testing.Data;
using EfEagerLoad.Testing.Extensions;
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

            await testRunner.RunTest1();
            await testRunner.RunTest2();
            //await testRunner.RunTest3();
            //await testRunner.RunTest4();
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
