using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkDoneTextDocumentRegistrationOptions : TextDocumentRegistrationOptions, IWorkDoneProgressOptions
    {
        /// <inheritdoc />
        [Optional]
        public bool WorkDoneProgress { get; set; }
    }
}
