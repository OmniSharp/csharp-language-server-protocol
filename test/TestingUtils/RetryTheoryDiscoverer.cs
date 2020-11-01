// See https://github.com/JoshKeegan/xRetry

using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestingUtils
{
    public class RetryTheoryDiscoverer : TheoryDiscoverer
    {
        public RetryTheoryDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink) {  }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForDataRow(
            ITestFrameworkDiscoveryOptions discoveryOptions,
            ITestMethod testMethod,
            IAttributeInfo theoryAttribute,
            object[] dataRow)
        {
            var maxRetries = theoryAttribute.GetNamedArgument<int>(nameof(RetryTheoryAttribute.MaxRetries));
            var delayBetweenRetriesMs =
                theoryAttribute.GetNamedArgument<int>(nameof(RetryTheoryAttribute.DelayBetweenRetriesMs));
            return new[]
            {
                new RetryTestCase(
                    DiagnosticMessageSink,
                    discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(),
                    testMethod,
                    maxRetries,
                    delayBetweenRetriesMs,
                    dataRow)
            };
        }

        protected override IEnumerable<IXunitTestCase> CreateTestCasesForTheory(
            ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo theoryAttribute)
        {
            var maxRetries = theoryAttribute.GetNamedArgument<int>(nameof(RetryTheoryAttribute.MaxRetries));
            var delayBetweenRetriesMs =
                theoryAttribute.GetNamedArgument<int>(nameof(RetryTheoryAttribute.DelayBetweenRetriesMs));

            return new[]
            {
                new RetryTheoryDiscoveryAtRuntimeCase(DiagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(),
                    discoveryOptions.MethodDisplayOptionsOrDefault(), testMethod, maxRetries, delayBetweenRetriesMs)
            };
        }
    }
}
