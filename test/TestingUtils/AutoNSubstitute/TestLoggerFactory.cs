using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Xunit.Abstractions;
using ILogger = Microsoft.Extensions.Logging.ILogger;

// ReSharper disable once CheckNamespace
namespace NSubstitute
{
    public class TestLoggerFactory : ILoggerFactory, IObservable<LogEvent>
    {
        private readonly SerilogLoggerProvider _loggerProvider;
        private readonly List<ILoggerProvider> _additionalLoggerProviders = new List<ILoggerProvider>();
        private readonly InnerTestOutputHelper _testOutputHelper;
        private readonly Subject<LogEvent> _subject;

        public TestLoggerFactory(
            ITestOutputHelper? testOutputHelper, string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
            LogEventLevel logEventLevel = LogEventLevel.Debug
        )
        {
            _subject = new Subject<LogEvent>();
            _testOutputHelper = new InnerTestOutputHelper(testOutputHelper);

            _loggerProvider = new SerilogLoggerProvider(
                new LoggerConfiguration()
                   .MinimumLevel.Is(logEventLevel)
                   .WriteTo.TestOutput(_testOutputHelper, outputTemplate: outputTemplate)
                   .WriteTo.Observers(x => x.Subscribe(_subject))
                   .Enrich.FromLogContext()
                   .CreateLogger()
            );
        }

        ILogger ILoggerFactory.CreateLogger(string categoryName)
        {
            return _additionalLoggerProviders.Count > 0
                ? new TestLogger(new [] { _loggerProvider }.Concat(_additionalLoggerProviders).Select(z => z.CreateLogger(categoryName)))
                : _loggerProvider.CreateLogger(categoryName);
        }

        void ILoggerFactory.AddProvider(ILoggerProvider provider)
        {
            _additionalLoggerProviders.Add(provider);
        }

        void IDisposable.Dispose()
        {
            _subject.Dispose();
        }

        public void Swap(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper.Swap(testOutputHelper);
        }

        private class InnerTestOutputHelper : ITestOutputHelper
        {
            private ITestOutputHelper? _testOutputHelper;

            public InnerTestOutputHelper(ITestOutputHelper? testOutputHelper)
            {
                _testOutputHelper = testOutputHelper;
            }

            public void Swap(ITestOutputHelper testOutputHelper)
            {
                Interlocked.Exchange(ref _testOutputHelper, testOutputHelper);
            }

            public void WriteLine(string message) => _testOutputHelper?.WriteLine(message);

            public void WriteLine(string format, params object[] args) => _testOutputHelper?.WriteLine(format, args);
        }

        public IDisposable Subscribe(IObserver<LogEvent> observer) => _subject.Subscribe(observer);
    }
}
