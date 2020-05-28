using System;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    public class TestLoggerFactory : ILoggerFactory
    {
        private readonly SerilogLoggerProvider _loggerProvider;

        public TestLoggerFactory(ITestOutputHelper testOutputHelper, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}", LogEventLevel logEventLevel = LogEventLevel.Debug)
        {
            _loggerProvider = new SerilogLoggerProvider(
                new LoggerConfiguration()
                    .MinimumLevel.Is(logEventLevel)
                    .WriteTo.TestOutput(testOutputHelper)
                    .CreateLogger()
            );
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName)
        {
            return _loggerProvider.CreateLogger(categoryName);
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider) { }

        void IDisposable.Dispose() { }
    }
}
