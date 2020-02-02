using System;
using System.Collections.Generic;
using System.Text;
using EfEagerLoad.ConsoleTester.Data;
using EfEagerLoad.ConsoleTester.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.ConsoleTester.Configuration
{
    public static class ServicesConfiguration
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddTransient(typeof(ILogger<>), typeof(ConsoleLogger<>));
            services.AddTransient(typeof(ILogger), typeof(ConsoleLogger));
            services.AddTransient(typeof(ConsoleLogger));
            services.AddTransient<ILoggerFactory, ConsoleLoggerFactory>();
            services.AddTransient<TestRunner>();
        }
    }
}
