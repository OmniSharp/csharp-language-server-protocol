// See https://github.com/JoshKeegan/xRetry

using System;
using System.Linq;
using Xunit;
using Xunit.Sdk;

namespace TestingUtils
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a fact that should be run
    /// by the test runner up to MaxRetries times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("TestingUtils.RetryFactDiscoverer", "TestingUtils")]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryFactAttribute : FactAttribute
    {
        public readonly int MaxRetries;
        public readonly int DelayBetweenRetriesMs;
        public readonly SkipOnPlatform[] PlatformsToSkip;
        private string? _skip;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="maxRetries">The number of times to run a test for until it succeeds</param>
        /// <param name="delayBetweenRetriesMs">The amount of time (in ms) to wait between each test run attempt</param>
        /// <param name="skipOn">platforms to skip testing on</param>
        public RetryFactAttribute(int maxRetries = 5, int delayBetweenRetriesMs = 0, params SkipOnPlatform[] skipOn)
        {
            if (maxRetries < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetries) + " must be >= 1");
            }
            if (delayBetweenRetriesMs < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(delayBetweenRetriesMs) + " must be >= 0");
            }

            MaxRetries = !UnitTestDetector.IsCI() ? 1 : maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
            PlatformsToSkip = skipOn;
        }

        public override string? Skip
        {
            get => UnitTestDetector.IsCI() && PlatformsToSkip.Any(UnitTestDetector.PlatformToSkipPredicate)
                ? "Skipped on platform" + ( string.IsNullOrWhiteSpace(_skip) ? "" : " because " + _skip )
                : null;
            set => _skip = value;
        }
    }
}
