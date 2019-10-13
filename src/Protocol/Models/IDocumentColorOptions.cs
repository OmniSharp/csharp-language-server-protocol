namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IDocumentColorOptions : IWorkDoneProgressOptions
    {
        bool ResolveProvider { get; set; }
    }
}
