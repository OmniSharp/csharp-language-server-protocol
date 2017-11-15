using OmniSharp.Extensions.LanguageServer.Capabilities.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Handlers
{
    /// <summary>
    ///     Handler for "client/registerCapability".
    /// </summary>
    /// <remarks>
    ///     For now, this handler does nothing other than a void reply; we don't support dynamic registrations yet.
    /// </remarks>
    public class DynamicRegistrationHandler
        : IInvokeRequestHandler
    {
        /// <summary>
        ///     Create a new <see cref="DynamicRegistrationHandler"/>.
        /// </summary>
        public DynamicRegistrationHandler()
        {
        }

        /// <summary>
        ///     Server capabilities dynamically updated by the handler.
        /// </summary>
        public ServerCapabilities ServerCapabilities { get; set; } = new ServerCapabilities();

        /// <summary>
        ///     The name of the method handled by the handler.
        /// </summary>
        public string Method => "client/registerCapability";

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
        public Task<object> Invoke(JObject request, CancellationToken cancellationToken)
        {
            // For now, we don't really support dynamic registration but OmniSharp's implementation sends a request even when dynamic registrations are not supported.

            return Task.FromResult<object>(null);
        }
    }
}
