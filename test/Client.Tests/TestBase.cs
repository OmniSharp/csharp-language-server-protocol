using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServer.Client.Tests
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
            LoggerFactory = new LoggerFactory();
            Disposal.Add(LoggerFactory);

            // LoggerFactory.AddDebug(LogLevel);
            LoggerFactory.AddTestOutput(TestOutput, LogLevel);

            // Ugly hack to get access to the current test.
            CurrentTest = (ITest)
                TestOutput.GetType()
                    .GetField("test", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(TestOutput);

            Assert.True(CurrentTest != null, "Cannot retrieve current test from ITestOutputHelper.");

            Log = LoggerFactory.CreateLogger("CurrentTest");

            Disposal.Add(
                Log.BeginScope("TestDisplayName='{TestName}'", CurrentTest.DisplayName)
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
        protected ILoggerFactory LoggerFactory { get; }

        /// <summary>
        ///     The Serilog logger for the current test.
        /// </summary>
        protected ILogger Log { get; }

        /// <summary>
        ///     The logging level for the current test.
        /// </summary>
        protected virtual LogLevel LogLevel => LogLevel.Information;

        /// <summary>
        ///     Is the test running on Windows?
        /// </summary>
        protected virtual bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        ///     Is the test running on a Unix-like operating system?
        /// </summary>
        protected virtual bool IsUnix => RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }
}
