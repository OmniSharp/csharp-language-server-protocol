using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client.Clients
{
    /// <summary>
    ///     Client for the LSP Text Document API.
    /// </summary>
    public partial class TextDocumentClient
    {
        /// <summary>
        ///     Create a new <see cref="TextDocumentClient"/>.
        /// </summary>
        /// <param name="client">
        ///     The language client providing the API.
        /// </param>
        public TextDocumentClient(LanguageClient client)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));

            Client = client;
        }

        /// <summary>
        ///     The language client providing the API.
        /// </summary>
        public LanguageClient Client { get; }

        /// <summary>
        ///     Make a request to the server for the specified document position.
        /// </summary>
        /// <typeparam name="TResponse">
        ///     The response payload type.
        /// </typeparam>
        /// <param name="method">
        ///     The name of the operation to invoke.
        /// </param>
        /// <param name="documentUri">
        ///     The URI of the target document.
        /// </param>
        /// <param name="line">
        ///     The target line numer (0-based).
        /// </param>
        /// <param name="column">
        ///     The target column (0-based).
        /// </param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> representing the request.
        /// </returns>
        async Task<TResponse> PositionalRequest<TResponse>(string method, DocumentUri documentUri, int line, int column, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(method))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(method)}.", nameof(method));

            if (documentUri == null)
                throw new ArgumentNullException(nameof(documentUri));

            var request = new TextDocumentPositionParams
            {
                TextDocument = new TextDocumentItem
                {
                    Uri = documentUri
                },
                Position = new Position
                {
                    Line = line,
                    Character = column
                }
            };

            return await Client.SendRequest<TResponse>(method, request, cancellationToken).ConfigureAwait(false);
        }
    }
}
