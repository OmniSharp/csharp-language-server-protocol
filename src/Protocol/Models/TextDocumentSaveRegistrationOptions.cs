using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class TextDocumentSaveRegistrationOptions : TextDocumentRegistrationOptions
    {
        /// <summary>
        ///  The client is supposed to include the content on save.
        /// </summary>
        [Optional]
        public bool IncludeText { get; set; }
    }
}
