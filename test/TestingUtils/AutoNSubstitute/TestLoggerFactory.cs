using System;
using System.Threading;
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
        private readonly InnerTestOutputHelper _testOutputHelper;

        public TestLoggerFactory(
            ITestOutputHelper testOutputHelper, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
            LogEventLevel logEventLevel = LogEventLevel.Debug
        )
        {
            _testOutputHelper = new InnerTestOutputHelper();
            _testOutputHelper.Swap(testOutputHelper);

            _loggerProvider = new SerilogLoggerProvider(
                new LoggerConfiguration()
                   .MinimumLevel.Is(logEventLevel)
                   .WriteTo.TestOutput(_testOutputHelper)
                   .CreateLogger()
            );
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName)
        {
            return _loggerProvider.CreateLogger(categoryName);
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider) { }

        void IDisposable.Dispose()
        {
        }

        public void Swap(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper.Swap(testOutputHelper);
        }

        class InnerTestOutputHelper : ITestOutputHelper
        {
            private ITestOutputHelper _testOutputHelper;
            public void Swap(ITestOutputHelper testOutputHelper)
            {
                Interlocked.Exchange(ref _testOutputHelper, testOutputHelper);
            }

            public void WriteLine(string message) => _testOutputHelper?.WriteLine(message);

            public void WriteLine(string format, params object[] args) => _testOutputHelper?.WriteLine(format, args);
        }
    }
}
