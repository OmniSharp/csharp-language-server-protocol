using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkDoneTextDocumentPositionParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <inheritdoc/>
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}