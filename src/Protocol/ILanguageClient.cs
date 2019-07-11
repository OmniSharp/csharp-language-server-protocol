using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public interface ILanguageClient : ILanguageClientRegistry, IResponseRouter
    {
        ILanguageClientDocument Document { get; }
        ILanguageClientClient Client { get; }
        ILanguageClientWindow Window { get; }
        ILanguageClientWorkspace Workspace { get; }
    }
}
