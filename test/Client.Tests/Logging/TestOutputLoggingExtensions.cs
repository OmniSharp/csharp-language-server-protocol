using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests.Logging
{
    /// <summary>
    ///     Extension methods for configuring logging to Xunit test output.
    /// </summary>
    public static class TestOutputLoggingExtensions
    {
        /// <summary>
        ///     Write log events to Xunit test output.
        /// </summary>
        /// <param name="loggerFactory">
        ///     The logger factory to configure.
        /// </param>
        /// <param name="testOutput">
        ///     The test output to which events will be logged.
        /// </param>
        /// <param name="minimumLevel">
        ///     The minimum level to log at.
        /// </param>
        public static void AddTestOutput(this ILoggerFactory loggerFactory, ITestOutputHelper testOutput, LogLevel minimumLevel = LogLevel.Information)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            loggerFactory.AddProvider(
                new TestOutputLoggingProvider(testOutput, minimumLevel)
            );
        }
    }
}
