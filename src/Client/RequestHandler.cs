using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    /// <summary>
    ///     A handler for requests.
    /// </summary>
    /// <typeparam name="TRequest">
    ///     The request message type.
    /// </typeparam>
    /// <param name="request">
    ///     The request message.
    /// </param>
    /// <param name="cancellationToken">
    ///     A <see cref="CancellationToken"/> that can be used to cancel the operation.
    /// </param>
    /// <returns>
    ///     A <see cref="Task"/> representing the operation.
    /// </returns>
    public delegate Task RequestHandler<TRequest>(TRequest request, CancellationToken cancellationToken);
}