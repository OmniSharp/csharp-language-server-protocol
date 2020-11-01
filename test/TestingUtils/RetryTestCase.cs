using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestingUtils
{
    [Serializable]
    public class RetryTestCase : XunitTestCase, IRetryableTestCase
    {
        public int MaxRetries { get; private set; }
        public int DelayBetweenRetriesMs { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(
            "Called by the de-serializer; should only be called by deriving classes for de-serialization purposes", true)]
        public RetryTestCase() { }

        public RetryTestCase(
            IMessageSink diagnosticMessageSink,
            TestMethodDisplay defaultMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod,
            int maxRetries,
            int delayBetweenRetriesMs,
            object[]? testMethodArguments = null)
            : base(diagnosticMessageSink, defaultMethodDisplay, defaultMethodDisplayOptions, testMethod,
                testMethodArguments)
        {
            MaxRetries = maxRetries;
            DelayBetweenRetriesMs = delayBetweenRetriesMs;
        }

        public override Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus,
            object[] constructorArguments, ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource) =>
            RetryTestCaseRunner.RunAsync(this, diagnosticMessageSink, messageBus, cancellationTokenSource,
                blockingMessageBus => new XunitTestCaseRunner(this, DisplayName, SkipReason, constructorArguments,
                        TestMethodArguments, blockingMessageBus, aggregator, cancellationTokenSource)
                    .RunAsync());

        public override void Serialize(IXunitSerializationInfo data)
        {
            base.Serialize(data);

            data.AddValue("MaxRetries", MaxRetries);
            data.AddValue("DelayBetweenRetriesMs", DelayBetweenRetriesMs);
        }

        public override void Deserialize(IXunitSerializationInfo data)
        {
            base.Deserialize(data);

            MaxRetries = data.GetValue<int>("MaxRetries");
            DelayBetweenRetriesMs = data.GetValue<int>("DelayBetweenRetriesMs");
        }
    }
}
