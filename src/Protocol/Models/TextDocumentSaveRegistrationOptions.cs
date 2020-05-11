using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TextDocumentSaveRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        ///  The client is supposed to include the content on save.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool IncludeText { get; set; }
    }
}
