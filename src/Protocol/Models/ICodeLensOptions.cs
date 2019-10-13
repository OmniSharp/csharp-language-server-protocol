namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface ICodeLensOptions : IWorkDoneProgressOptions
    {
        bool ResolveProvider { get; set; }
    }
}
