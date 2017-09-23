namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    public interface IExecuteCommandOptions
    {
        Container<string> Commands { get; set; }
    }
}