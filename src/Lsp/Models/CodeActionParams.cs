using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    public interface ITextDocumentIdentifierParams { TextDocumentIdentifier TextDocument { get; } }

    /// <summary>
    /// Params for the CodeActionRequest
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CodeActionParams : ITextDocumentIdentifierParams
    {
        /// <summary>
        /// The document in which the command was invoked.
        /// </summary>
        public TextDocumentIdentifier TextDocument { get; set; }

        /// <summary>
        /// The range for which the command was invoked.
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Context carrying additional information.
        /// </summary>
        public CodeActionContext Context { get; set; }
    }
}