namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ITextDocumentRegistrationOptions : IRegistrationOptions
    {
        DocumentSelector? DocumentSelector { get; set; }
    }
}
