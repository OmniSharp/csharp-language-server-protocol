using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests.Logging
{
    /// <summary>
    ///     A logger that writes to Xunit test output.
    /// </summary>
    public class TestOutputLogger
        : ILogger
    {
        /// <summary>
        ///     The Xunit test output.
        /// </summary>
        readonly ITestOutputHelper _testOutput;

        /// <summary>
        ///     The logger name.
        /// </summary>
        readonly string _name;

        /// <summary>
        ///     The minimum level to log at.
        /// </summary>
        readonly LogLevel _minimumLevel;

        /// <summary>
        ///     Create a new <see cref="TestOutputLogger"/>.
        /// </summary>
        /// <param name="testOutput">
        ///     The Xunit test output.
        /// </param>
        /// <param name="name">
        ///     The logger name.
        /// </param>
        /// <param name="minimumLevel">
        ///     The minimum level to log at.
        /// </param>
        public TestOutputLogger(ITestOutputHelper testOutput, string name, LogLevel minimumLevel)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            if (name == null)
                throw new ArgumentNullException(nameof(name));

            _testOutput = testOutput;

            // Trim off namespace, if possible.
            string[] nameSegments = name.Split('.');
            _name = nameSegments[nameSegments.Length - 1];

            _minimumLevel = minimumLevel;
        }

        /// <summary>
        ///     Begin a new log scope.
        /// </summary>
        /// <typeparam name="TState">
        ///     The type used as state for the scope.
        /// </typeparam>
        /// <param name="state">
        ///     The scope state.
        /// </param>
        /// <returns>
        ///     An <see cref="IDisposable"/> representing the scope.
        /// </returns>
        public IDisposable BeginScope<TState>(TState state) => TestOutputLogScope.Push(_name, state);

        /// <summary>
        ///     Determine whether logging is enabled at the specified level.
        /// </summary>
        /// <param name="logLevel">
        ///     The target log level.
        /// </param>
        /// <returns>
        ///     <c>true</c>, if logging is enabled; otherwise, <c>false</c>.
        /// </returns>
        public bool IsEnabled(LogLevel logLevel) => logLevel >= _minimumLevel;

        /// <summary>
        ///     Write a log message to the test output.
        /// </summary>
        /// <typeparam name="TState">
        ///     The type used as state for the log entry.
        /// </typeparam>
        /// <param name="logLevel">
        ///     The log entry's associated logging level.
        /// </param>
        /// <param name="eventId">
        ///     An <see cref="EventId"/> identifying the log entry type.
        /// </param>
        /// <param name="state">
        ///     The log entry state.
        /// </param>
        /// <param name="exception">
        ///     The <see cref="Exception"/> (if any) associated with the log entry.
        /// </param>
        /// <param name="formatter">
        ///     A delegate that formats the log message.
        /// </param>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel < _minimumLevel)
                return;

            string prefix = !string.IsNullOrWhiteSpace(_name)
                ? $"[{_name}/{logLevel}] "
                : $"[{logLevel}] ";

            string message = prefix + formatter(state, exception);
            if (exception != null)
                message += "\n" + exception.ToString();

            try
            {
                _testOutput.WriteLine(message);
            }
            catch (InvalidOperationException)
            {
                // Test has already terminated.

                System.Diagnostics.Debug.WriteLine(message);
            }
        }
    }
}
