namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class DocumentOnTypeFormattingRegistrationOptions : WorkDoneTextDocumentRegistrationOptions, IDocumentOnTypeFormattingOptions
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
