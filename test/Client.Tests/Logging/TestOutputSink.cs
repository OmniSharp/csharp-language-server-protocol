using Serilog.Core;
using System;
using System.Linq;
using Serilog.Events;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests.Logging
{
    /// <summary>
    ///     A Serilog logging sink that sends log events to Xunit test output.
    /// </summary>
    public class TestOutputLoggingSink
        : ILogEventSink
    {
        /// <summary>
        ///     The test output to which events will be logged.
        /// </summary>
        readonly ITestOutputHelper _testOutput;

        /// <summary>
        ///     The <see cref="LoggingLevelSwitch"/> that controls logging.
        /// </summary>
        readonly LoggingLevelSwitch _levelSwitch;

        /// <summary>
        ///     Create a new test output event sink.
        /// </summary>
        /// <param name="testOutput">
        ///     The test output to which events will be logged.
        /// </param>
        /// <param name="levelSwitch">
        ///     The <see cref="LoggingLevelSwitch"/> that controls logging.
        /// </param>
        public TestOutputLoggingSink(ITestOutputHelper testOutput, LoggingLevelSwitch levelSwitch)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            if (levelSwitch == null)
                throw new ArgumentNullException(nameof(levelSwitch));

            _testOutput = testOutput;
            _levelSwitch = levelSwitch;
        }

        /// <summary>
        ///     Emit a log event.
        /// </summary>
        /// <param name="logEvent">
        ///     The log event information.
        /// </param>
        public void Emit(LogEvent logEvent)
        {
            if (logEvent.Level < _levelSwitch.MinimumLevel)
                return;

            string sourceContext = String.Empty;
            if (logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue sourceContextValue))
            {
                if (sourceContextValue is ScalarValue scalarValue)
                    sourceContext = scalarValue.Value?.ToString() ?? String.Empty;
                else
                    sourceContext = sourceContextValue.ToString();
            }

            // Trim off namespace, if possible.
            string[] sourceContextSegments = sourceContext.Split('.');
            sourceContext = sourceContextSegments[sourceContextSegments.Length - 1];

            string prefix = !String.IsNullOrWhiteSpace(sourceContext)
                ? $"[{sourceContext}/{logEvent.Level}] "
                : $"[{logEvent.Level}] ";

            string message = prefix + logEvent.RenderMessage();
            if (logEvent.Exception != null)
                message += "\n" + logEvent.Exception.ToString();

            _testOutput.WriteLine(message);
        }
    }
}
