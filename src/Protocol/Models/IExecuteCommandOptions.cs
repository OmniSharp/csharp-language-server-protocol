namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IExecuteCommandOptions : IWorkDoneProgressOptions
    {
        Container<string> Commands { get; set; }
    }
}
