namespace OmniSharp.Extensions.LanguageServer.Models
{
    public interface ISignatureHelpOptions
    {
        Container<string> TriggerCharacters { get; set; }
    }
}
