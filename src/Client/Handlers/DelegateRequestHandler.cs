using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Client.Handlers
{
    /// <summary>
    ///     A delegate-based handler for requests whose responses have no payload (i.e. void return type).
    /// </summary>
    /// <typeparam name="TRequest">
    ///     The request message type.
    /// </typeparam>
    public class DelegateRequestHandler<TRequest>
        : DelegateHandler, IInvokeRequestHandler
    {
        /// <summary>
        ///     Create a new <see cref="DelegateRequestHandler{TRequest}"/>.
        /// </summary>
        /// <param name="method">
        ///     The name of the method handled by the handler.
        /// </param>
        /// <param name="handler">
        ///     The <see cref="RequestHandler{TRequest}"/> delegate that implements the handler.
        /// </param>
        public DelegateRequestHandler(string method, RequestHandler<TRequest> handler)
            : base(method)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Handler = handler;
        }

        /// <summary>
        ///     The <see cref="RequestHandler{TRequest}"/> delegate that implements the handler.
        /// </summary>
        public RequestHandler<TRequest> Handler { get; }

        /// <summary>
        ///     The expected CLR type of the request body.
        /// </summary>
        public override Type BodyType => typeof(TRequest);

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
        public async Task<object> Invoke(object request, CancellationToken cancellationToken)
        {
            await Handler(
                (TRequest)request,
                cancellationToken
            );

            return null;
        }
    }
}
