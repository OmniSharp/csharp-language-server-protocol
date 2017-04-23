namespace Lsp.Models
{
    public interface ISignatureHelpOptions
    {
        Container<string> TriggerCharacters { get; set; }
    }
}