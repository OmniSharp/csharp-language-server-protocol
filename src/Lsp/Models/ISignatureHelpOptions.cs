namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    public interface ISignatureHelpOptions
    {
        Container<string> TriggerCharacters { get; set; }
    }
}