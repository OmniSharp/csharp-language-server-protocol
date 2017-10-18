using System;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Abstractions;
using Serilog;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Lsp.Tests
{
    public class TestLoggerFactory : ILoggerFactory
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly SerilogLoggerProvider _loggerProvider;

        public TestLoggerFactory(ITestOutputHelper testOutputHelper)
        {
            _loggerProvider = new SerilogLoggerProvider(
                new Serilog.LoggerConfiguration()
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
