using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.ConsoleTester.Logging
{
    public class ConsoleLogger : ILogger
    {
        private static LogLevel CurrentLogLevel = LogLevel.Information;
        private static int _commandCounter = 0;

        public static int CommandCounter => _commandCounter;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) { return; }

            var message = formatter(state, exception);
            if (message.Contains("Executed DbCommand"))
            {
                Interlocked.Increment(ref _commandCounter);
            }
            Console.WriteLine(message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= CurrentLogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
