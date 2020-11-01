// See https://github.com/JoshKeegan/xRetry

using Xunit.Sdk;

namespace TestingUtils
{
    public interface IRetryableTestCase : IXunitTestCase
    {
        int MaxRetries { get; }
        int DelayBetweenRetriesMs { get; }
    }
}
