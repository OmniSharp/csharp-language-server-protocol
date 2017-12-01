namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IDocumentOnTypeFormattingOptions
    {
        string FirstTriggerCharacter { get; set; }
        Container<string> MoreTriggerCharacter { get; set; }
    }
}