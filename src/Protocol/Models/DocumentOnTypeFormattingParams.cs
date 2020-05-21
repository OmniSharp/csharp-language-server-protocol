using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Method(TextDocumentNames.OnTypeFormatting, Direction.ClientToServer)]
    public class DocumentOnTypeFormattingParams : ITextDocumentIdentifierParams, IRequest<TextEditContainer>
    {
        /// <summary>
        /// The document to format.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The position at which this request was sent.
        /// </summary>
        public Position Position { get; set; }

        /// <summary>
        /// The character that has been typed.
        /// </summary>
        [JsonProperty("ch")]
        public string Character { get; set; }

        /// <summary>
        /// The format options.
        /// </summary>
        public FormattingOptions Options { get; set; }
    }
}
