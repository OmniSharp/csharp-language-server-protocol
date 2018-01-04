using OmniSharp.Extensions.LanguageServer.Client.Handlers;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Tests
{
    /// <summary>
    ///     Tests for <see cref="IHandler"/> and friends.
    /// </summary>
    public class HandlerTests
        : TestBase
    {
        /// <summary>
        ///     Create a new <see cref="IHandler"/> test suite.
        /// </summary>
        /// <param name="testOutput">
        ///     Output for the current test.
        /// </param>
        public HandlerTests(ITestOutputHelper testOutput)
            : base(testOutput)
        {
        }

        /// <summary>
        ///     Verify that <see cref="DelegateEmptyNotificationHandler"/> specifies the correct payload type.
        /// </summary>
        [Fact(DisplayName = "DelegateEmptyNotificationHandler specifies correct payload type")]
        public void DelegateEmptyNotificationHandler_PayloadType()
        {
            IHandler handler = new DelegateEmptyNotificationHandler(
                method: "test",
                handler: () =>
                {
                    // Nothing to do.
                }
            );

            Assert.Null(handler.PayloadType);
        }

        /// <summary>
        ///     Verify that <see cref="DelegateNotificationHandler"/> specifies the correct payload type.
        /// </summary>
        [Fact(DisplayName = "DelegateNotificationHandler specifies correct payload type")]
        public void DelegateNotificationHandler_PayloadType()
        {
            IHandler handler = new DelegateNotificationHandler<string>(
                method: "test",
                handler: notification =>
                {
                    // Nothing to do.
                }
            );

            Assert.Equal(typeof(string), handler.PayloadType);
        }

        /// <summary>
        ///     Verify that <see cref="DelegateRequestHandler{TRequest}"/> specifies the correct payload type (<c>null</c>).
        /// </summary>
        [Fact(DisplayName = "DelegateRequestHandler specifies correct payload type")]
        public void DelegateRequestHandler_PayloadType()
        {
            IHandler handler = new DelegateRequestHandler<string>(
                method: "test",
                handler: (request, cancellationToken) =>
                {
                    // Nothing to do.

                    return Task.CompletedTask;
                }
            );

            Assert.Equal(typeof(string), handler.PayloadType);
        }

        /// <summary>
        ///     Verify that <see cref="DelegateRequestResponseHandler{TRequest, TResponse}"/> specifies the correct payload type (<c>null</c>).
        /// </summary>
        [Fact(DisplayName = "DelegateRequestResponseHandler specifies correct payload type")]
        public void DelegateRequestResponseHandler_PayloadType()
        {
            IHandler handler = new DelegateRequestResponseHandler<string, string>(
                method: "test",
                handler: (request, cancellationToken) =>
                {
                    // Nothing to do.

                    return Task.FromResult<string>("hello");
                }
            );

            Assert.Equal(typeof(string), handler.PayloadType);
        }
    }
}
