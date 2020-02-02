using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.ConsoleTester.Logging
{
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new ConsoleLogger<object>(new ConsoleLogger());
        }

        public void AddProvider(ILoggerProvider provider)
        {
        }
    }
}
