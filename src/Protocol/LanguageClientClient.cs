using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class LanguageClientClient : ClientProxyBase, ILanguageClientClient
    {
        public LanguageClientClient(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
