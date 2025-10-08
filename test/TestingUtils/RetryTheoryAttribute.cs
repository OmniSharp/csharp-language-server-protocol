// See https://github.com/JoshKeegan/xRetry

using Xunit.Sdk;

namespace TestingUtils
{
    /// <summary>
    /// Attribute that is applied to a method to indicate that it is a theory that should be run
    /// by the test runner up to MaxRetries times, until it succeeds.
    /// </summary>
    [XunitTestCaseDiscoverer("TestingUtils.RetryTheoryDiscoverer", "TestingUtils")]
    [AttributeUsage(AttributeTargets.Method)]
    public class RetryTheoryAttribute : RetryFactAttribute
    {
        /// <inheritdoc />
        public RetryTheoryAttribute(int maxRetries = 3, int delayBetweenRetriesMs = 0, params SkipOnPlatform[] skipOn) : base(
            maxRetries, delayBetweenRetriesMs, skipOn
        )
        {
        }
    }
}
