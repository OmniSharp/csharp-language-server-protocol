using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public abstract class WorkDoneTextDocumentPositionParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <inheritdoc/>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
