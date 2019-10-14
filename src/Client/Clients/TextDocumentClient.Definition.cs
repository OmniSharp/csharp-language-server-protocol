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
        ///     Request definition at the specified document position.
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
        public Task<LocationOrLocationLinks> Definition(string filePath, int line, int column, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException($"Argument cannot be null, empty, or entirely composed of whitespace: {nameof(filePath)}.", nameof(filePath));

            var documentUri = DocumentUri.FromFileSystemPath(filePath);

            return Definition(documentUri, line, column, cancellationToken);
        }

        /// <summary>
        ///     Request definition at the specified document position.
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
        public Task<LocationOrLocationLinks> Definition(Uri documentUri, int line, int column, CancellationToken cancellationToken = default(CancellationToken))
        {
            return PositionalRequest<LocationOrLocationLinks>(DocumentNames.Definition, documentUri, line, column, cancellationToken);
        }
    }
}
