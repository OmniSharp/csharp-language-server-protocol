using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using System;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests.Logging
{
    /// <summary>
    ///     Extension methods for configuring Serilog.
    /// </summary>
    public static class SerilogTestOutputExtensions
    {
        /// <summary>
        ///     Write log events to Xunit test output.
        /// </summary>
        /// <param name="loggerSinkConfiguration">
        ///     The logger sink configuration.
        /// </param>
        /// <param name="testOutput">
        ///     The test output to which events will be logged.
        /// </param>
        /// <param name="levelSwitch">
        ///     An optional <see cref="LoggingLevelSwitch"/> to control logging.
        /// </param>
        /// <returns>
        ///     The logger configuration.
        /// </returns>
        public static LoggerConfiguration TestOutput(this LoggerSinkConfiguration loggerSinkConfiguration, ITestOutputHelper testOutput, LoggingLevelSwitch levelSwitch = null)
        {
            if (loggerSinkConfiguration == null)
                throw new ArgumentNullException(nameof(loggerSinkConfiguration));
            
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            return loggerSinkConfiguration.Sink(
                new TestOutputLoggingSink(testOutput,
                    levelSwitch: levelSwitch ?? new LoggingLevelSwitch()
                )
            );
        }
    }
}
