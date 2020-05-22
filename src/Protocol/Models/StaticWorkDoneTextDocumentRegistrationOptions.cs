using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Static registration options to be returned in the initialize request.
    /// </summary>
    public class StaticWorkDoneTextDocumentRegistrationOptions : StaticTextDocumentRegistrationOptions, IWorkDoneProgressOptions
    {
        [Optional]
        /// <inheritdoc />
        public bool WorkDoneProgress { get; set; }
    }
}
