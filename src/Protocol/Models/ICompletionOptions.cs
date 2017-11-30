namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ICompletionOptions
    {
        bool ResolveProvider { get; set; }
        Container<string> TriggerCharacters { get; set; }
    }
}