// See https://github.com/JoshKeegan/xRetry

using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TestingUtils
{
    internal static class RetryTestCaseRunner
    {
        /// <summary>
        /// Runs a retryable test case, handling any wait and retry logic between test runs, reporting statuses out to xunit etc...
        /// </summary>
        /// <param name="testCase">The test case to be retried</param>
        /// <param name="diagnosticMessageSink">The diagnostic message sink to write messages to about retries, waits etc...</param>
        /// <param name="messageBus">The message bus xunit is listening for statuses to report on</param>
        /// <param name="cancellationTokenSource">The cancellation token source from xunit</param>
        /// <param name="fnRunSingle">(async) Lambda to run this test case once (without retries) - takes the blocking message bus and returns the test run result</param>
        /// <returns>Resulting run summary</returns>
        public static async Task<RunSummary> RunAsync(
            IRetryableTestCase testCase,
            IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            CancellationTokenSource cancellationTokenSource,
            Func<IMessageBus, Task<RunSummary>> fnRunSingle)
        {
            for (var i = 1; ; i++)
            {
                // Prevent messages from the test run from being passed through, as we don't want
                //  a message to mark the test as failed when we're going to retry it
                using BlockingMessageBus blockingMessageBus = new BlockingMessageBus(messageBus);
                diagnosticMessageSink.OnMessage(new DiagnosticMessage("Running test \"{0}\" attempt ({1}/{2})",
                                                                      testCase.DisplayName, i, testCase.MaxRetries));

                RunSummary summary = await fnRunSingle(blockingMessageBus);

                // If we succeeded, or we've reached the max retries return the result
                if (summary.Failed == 0 || i == testCase.MaxRetries)
                {
                    // If we have failed (after all retries, log that)
                    if (summary.Failed != 0)
                    {
                        diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                                                            "Test \"{0}\" has failed and been retried the maximum number of times ({1})",
                                                            testCase.DisplayName, testCase.MaxRetries));
                    }

                    blockingMessageBus.Flush();
                    return summary;
                }
                // Otherwise log that we've had a failed run and will retry
                diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                                                    "Test \"{0}\" failed but is set to retry ({1}/{2}) . . .", testCase.DisplayName, i,
                                                    testCase.MaxRetries));

                // If there is a delay between test attempts, apply it now
                if (testCase.DelayBetweenRetriesMs > 0)
                {
                    diagnosticMessageSink.OnMessage(new DiagnosticMessage(
                                                        "Test \"{0}\" attempt ({1}/{2}) delayed by {3}ms. Waiting . . .", testCase.DisplayName, i,
                                                        testCase.MaxRetries, testCase.DelayBetweenRetriesMs));

                    // Don't await to prevent thread hopping.
                    //  If all of a users test cases in a collection/class are synchronous and expecting to not thread-hop
                    //  (because they're making use of thread static/thread local/managed thread ID to share data between tests rather than
                    //  a more modern async-friendly mechanism) then if a thread-hop were to happen here we'd get flickering tests.
                    //  SpecFlow relies on this as they use the managed thread ID to separate instances of some of their internal classes, which caused
                    //  a this problem for xRetry.SpecFlow: https://github.com/JoshKeegan/xRetry/issues/18
                    Task.Delay(testCase.DelayBetweenRetriesMs, cancellationTokenSource.Token).Wait();
                }
            }
        }
    }
}
