using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class LanguageServerClient : ClientProxyBase, ILanguageServerClient
    {
        public LanguageServerClient(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
