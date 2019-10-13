using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class RenameRegistrationOptions : WorkDoneTextDocumentRegistrationOptions, IRenameOptions
    {
        /// <summary>
        /// Renames should be checked and tested before being executed.
        /// </summary>
        [Optional]
        public bool PrepareProvider { get; set; }
    }
}
