using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Castle.DynamicProxy.Contributors;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    class TestLogger : ILogger
    {
        private readonly IEnumerable<ILogger> _loggers;

        public TestLogger(IEnumerable<ILogger> loggers)
        {
            _loggers = loggers;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            foreach (var logger in _loggers)
            {
                logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _loggers.Any(logger => logger.IsEnabled(logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new CompositeDisposable(_loggers.Select(z => z.BeginScope(state)));
        }
    }
}
