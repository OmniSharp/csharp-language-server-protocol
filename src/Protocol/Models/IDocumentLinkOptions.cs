namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IDocumentLinkOptions : IWorkDoneProgressOptions
    {
        bool ResolveProvider { get; set; }
    }
}
