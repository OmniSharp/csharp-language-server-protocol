namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ISignatureHelpOptions
    {
        Container<string> TriggerCharacters { get; set; }
    }
}
