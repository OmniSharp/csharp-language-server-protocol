using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class LanguageServerDocument : ClientProxyBase, ILanguageServerDocument
    {
        public LanguageServerDocument(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
