using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     A delegate-based handler for requests whose responses have payloads.
    /// </summary>
    /// <typeparam name="TRequest">
    ///     The request message type.
    /// </typeparam>
    /// <typeparam name="TResponse">
    ///     The response message type.
    /// </typeparam>
    public class DelegateRequestResponseHandler<TRequest, TResponse>
        : DelegateHandler, IInvokeRequestHandler
    {
        /// <summary>
        ///     Create a new <see cref="DelegateRequestResponseHandler{TRequest, TResponse}"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        /// <param name="handler">
        ///     The <see cref="RequestHandler{TRequest, TResponse}"/> delegate that implements the handler.
        /// </param>
        public DelegateRequestResponseHandler(string method, RequestHandler<TRequest, TResponse> handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The <see cref="RequestHandler{TRequest, TResponse}"/> delegate that implements the handler.
        /// </summary>
        public RequestHandler<TRequest, TResponse> Handler { get; }

        /// <summary>
        ///     The kind of handler.
        /// </summary>
        public override HandlerKind Kind => HandlerKind.Request;

        /// <summary>
        ///     Invoke the handler.
        /// </summary>
        /// <param name="request">
        ///     The request message.
        /// </param>
        /// <param name="cancellationToken">
        ///     A <see cref="CancellationToken"/> that can be used to cancel the operation.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> representing the operation.
        /// </returns>
        public async Task<object> Invoke(JObject request, CancellationToken cancellationToken)
        {
            return await Handler(
                request != null ? request.ToObject<TRequest>() : default(TRequest),
                cancellationToken
            );
        }
    }
}
