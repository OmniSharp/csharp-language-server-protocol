using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestingUtils
{
    public class RetryFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _messageSink;

        public RetryFactDiscoverer(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod,
            IAttributeInfo factAttribute)
        {
            IXunitTestCase testCase;

            if (testMethod.Method.GetParameters().Any())
            {
                testCase = new ExecutionErrorTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,
                    "[Fact]//[RetryFact] methods are not allowed to have parameters. Did you mean to use [RetryTheory]?");
            }
            else if (testMethod.Method.IsGenericMethodDefinition)
            {
                testCase = new ExecutionErrorTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod,
                    "[Fact]//[RetryFact] methods are not allowed to be generic.");
            }
            else
            {
                var maxRetries = factAttribute.GetNamedArgument<int>(nameof(RetryFactAttribute.MaxRetries));
                var delayBetweenRetriesMs =
                    factAttribute.GetNamedArgument<int>(nameof(RetryFactAttribute.DelayBetweenRetriesMs));
                testCase = new RetryTestCase(_messageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, maxRetries, delayBetweenRetriesMs);
            }

            return new[] { testCase };
        }
    }
}
