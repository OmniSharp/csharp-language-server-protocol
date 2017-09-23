namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    public interface ITextDocumentRegistrationOptions
    {
        DocumentSelector DocumentSelector { get; set; }
    }
}