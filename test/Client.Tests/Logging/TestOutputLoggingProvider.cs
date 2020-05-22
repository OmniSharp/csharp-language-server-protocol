using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;
using System.Collections.Concurrent;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests.Logging
{
    /// <summary>
    ///     A provider for loggers that send log events to Xunit test output.
    /// </summary>
    public sealed class TestOutputLoggingProvider
        : ILoggerProvider
    {
        /// <summary>
        ///     Loggers created by the provider.
        /// </summary>
        static readonly ConcurrentDictionary<string, TestOutputLogger> _loggers = new ConcurrentDictionary<string, TestOutputLogger>();

        /// <summary>
        ///     The test output to which events will be logged.
        /// </summary>
        readonly ITestOutputHelper _testOutput;

        /// <summary>
        ///     The minimum level to log at.
        /// </summary>
        readonly LogLevel _minimumLevel;

        /// <summary>
        ///     Create a new test output event sink.
        /// </summary>
        /// <param name="testOutput">
        ///     The test output to which events will be logged.
        /// </param>
        /// <param name="minimumLevel">
        ///     The minimum level to log at.
        /// </param>
        public TestOutputLoggingProvider(ITestOutputHelper testOutput, LogLevel minimumLevel)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            _testOutput = testOutput;
            _minimumLevel = minimumLevel;
        }

        /// <summary>
        ///     Create a new logger.
        /// </summary>
        /// <param name="name">
        ///     The logger name.
        /// </param>
        /// <returns>
        ///     The logger.
        /// </returns>
        public ILogger CreateLogger(string name)
        {
            if (name == null)
                name = string.Empty;

            return _loggers.GetOrAdd(name,
                new TestOutputLogger(_testOutput, name, _minimumLevel)
            );
        }

        /// <summary>
        ///     Dispose of resources being used by the logger provider and its loggers.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
