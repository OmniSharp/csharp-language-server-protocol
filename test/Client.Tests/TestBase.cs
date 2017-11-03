using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    using Logging;

    /// <summary>
    ///     The base class for test suites.
    /// </summary>
    public abstract class TestBase
        : IDisposable
    {
        /// <summary>
        ///     Create a new test-suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        protected TestBase(ITestOutputHelper testOutput)
        {
            if (testOutput == null)
                throw new ArgumentNullException(nameof(testOutput));

            // We *must* have a synchronisation context for the test, or we'll see random deadlocks.
            SynchronizationContext.SetSynchronizationContext(
                new SynchronizationContext()
            );

            TestOutput = testOutput;

            // Redirect component logging to Serilog.
            Log =
                new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .Enrich.FromLogContext()
                    .WriteTo.Debug(
                        restrictedToMinimumLevel: LogEventLevel.Verbose
                    )
                    .WriteTo.TestOutput(TestOutput, LogLevelSwitch)
                    .CreateLogger();

            // Ugly hack to get access to the current test.
            CurrentTest = (ITest)
                TestOutput.GetType()
                    .GetField("test", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(TestOutput);

            Assert.True(CurrentTest != null, "Cannot retrieve current test from ITestOutputHelper.");

            Disposal.Add(
                LogContext.PushProperty("TestName", CurrentTest.DisplayName)
            );
        }

        /// <summary>
        ///     Finaliser for <see cref="PipeServerTestBase"/>.
        /// </summary>
        ~TestBase()
        {
            Dispose(false);
        }

        /// <summary>
        ///     Dispose of resources being used by the test suite.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Dispose of resources being used by the test suite.
        /// </summary>
        /// <param name="disposing">
        ///     Explicit disposal?
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    Disposal.Dispose();
                }
                finally
                {
                    if (Log is IDisposable logDisposal)
                        logDisposal.Dispose();
                }
            }
        }

        /// <summary>
        ///     A <see cref="CompositeDisposable"/> representing resources used by the test.
        /// </summary>
        protected CompositeDisposable Disposal { get; } = new CompositeDisposable();

        /// <summary>
        ///     Output for the current test.
        /// </summary>
        protected ITestOutputHelper TestOutput { get; }

        /// <summary>
        ///     A <see cref="ITest"/> representing the current test.
        /// </summary>
        protected ITest CurrentTest { get; }

        /// <summary>
        ///     The Serilog logger for the current test.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        ///     A switch to control the logging level for the current test.
        /// </summary>
        protected LoggingLevelSwitch LogLevelSwitch { get; } = new LoggingLevelSwitch();
    }
}
