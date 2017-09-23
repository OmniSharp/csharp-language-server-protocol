namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    public interface IDocumentOnTypeFormattingOptions
    {
        string FirstTriggerCharacter { get; set; }
        Container<string> MoreTriggerCharacter { get; set; }
    }
}