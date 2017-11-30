namespace OmniSharp.Extensions.LanguageServer.Models
{
    public interface IExecuteCommandOptions
    {
        Container<string> Commands { get; set; }
    }
}