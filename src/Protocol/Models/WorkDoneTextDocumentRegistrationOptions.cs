using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkDoneTextDocumentRegistrationOptions : TextDocumentRegistrationOptions, IWorkDoneProgressOptions
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        /// <inheritdoc />
        public bool WorkDoneProgress { get; set; }
    }
}
