using OmniSharp.Extensions.LanguageServer.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.LanguageServerProtocol.Client.Clients
{
    using Utilities;

    /// <summary>
    ///     Client for the LSP Text Document API.
    /// </summary>
    public partial class TextDocumentClient
    {
        /// <summary>
        ///     Request completions at the specified document position.
        /// </summary>
        /// <param name="filePath">
        ///     The full file-system path of the text document.
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
        ///     A <see cref="Task{TResult}"/> that resolves to the completions or <c>null</c> if no completions are available at the specified position.
        /// </returns>
        public Task<CompletionList> Completions(string filePath, int line, int column, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (String.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            return PositionalRequest<CompletionList>("textDocument/completion", filePath, line, column, cancellationToken);
        }

        /// <summary>
        ///     Request completions at the specified document position.
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
        ///     A <see cref="Task{TResult}"/> that resolves to the completions or <c>null</c> if no completions are available at the specified position.
        /// </returns>
        public Task<CompletionList> Completions(Uri documentUri, int line, int column, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PositionalRequest<CompletionList>("textDocument/completion", documentUri, line, column, cancellationToken);
        }
    }
}
