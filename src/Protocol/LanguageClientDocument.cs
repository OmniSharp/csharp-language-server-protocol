using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class LanguageClientDocument : ClientProxyBase, ILanguageClientDocument
    {
        public LanguageClientDocument(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
