using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.ConsoleTester.Logging
{
    public class ConsoleLogger<T> : ILogger<T>
    {
        private readonly ConsoleLogger _wrappedConsoleLogger;

        public ConsoleLogger(ConsoleLogger wrappedConsoleLogger)
        {
            _wrappedConsoleLogger = wrappedConsoleLogger;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _wrappedConsoleLogger.Log<TState>(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _wrappedConsoleLogger.IsEnabled(logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _wrappedConsoleLogger.BeginScope(state);
        }
    }
}
