namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IExecuteCommandOptions
    {
        Container<string> Commands { get; set; }
    }
}