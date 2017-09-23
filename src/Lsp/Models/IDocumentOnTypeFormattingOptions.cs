namespace OmniSharp.Extensions.LanguageServer.Models
{
    public interface IDocumentOnTypeFormattingOptions
    {
        string FirstTriggerCharacter { get; set; }
        Container<string> MoreTriggerCharacter { get; set; }
    }
}