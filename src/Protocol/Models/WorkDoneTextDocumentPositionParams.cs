using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public abstract class WorkDoneTextDocumentPositionParams : TextDocumentPositionParams, IWorkDoneProgressParams
    {
        /// <inheritdoc/>
        [Optional]
        public ProgressToken WorkDoneToken { get; set; }
    }
}
