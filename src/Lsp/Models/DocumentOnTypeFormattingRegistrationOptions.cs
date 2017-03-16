using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lsp.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class DocumentOnTypeFormattingRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        /// A character on which formatting should be triggered, like `}`.
        /// </summary>
        public string FirstTriggerCharacter { get; set; }
        /// <summary>
        /// More trigger characters.
        /// </summary>
        public Container<string> MoreTriggerCharacter { get; set; }
    }
}