using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Client.Utilities;
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
        ///     Request document folding ranges.
        /// </summary>
        /// <param name="filePath">
        ///     The full file-system path of the text document.
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional <see cref="CancellationToken"/> that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> that resolves to the completions or <c>null</c> if no document highlights are available at the specified position.
        /// </returns>
        public Task<Container<FoldingRange>> FoldingRanges(string filePath, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            var documentUri = DocumentUri.FromFileSystemPath(filePath);

            return FoldingRanges(documentUri, cancellationToken);
        }

        /// <summary>
        ///     Request document highlights at the specified document position.
        /// </summary>
        /// <param name="documentUri">
        ///     The document URI.
        /// </param>
        /// <param name="line">
        ///     The target line (0-based).
        /// </param>
        /// <param name="column">
        ///     The target column (0-based).
        /// </param>
        /// <param name="cancellationToken">
        ///     An optional <see cref="CancellationToken"/> that can be used to cancel the request.
        /// </param>
        /// <returns>
        ///     A <see cref="Task{TResult}"/> that resolves to the completions or <c>null</c> if no document highlights are available at the specified position.
        /// </returns>
        public async Task<Container<FoldingRange>> FoldingRanges(Uri documentUri, CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new FoldingRangeRequestParam {
                TextDocument = new TextDocumentItem {
                    Uri = documentUri
                }
            };

            return await Client.SendRequest<Container<FoldingRange>>(DocumentNames.FoldingRange, request, cancellationToken).ConfigureAwait(false);
        }
    }
}
