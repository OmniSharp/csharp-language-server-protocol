using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class LanguageClientWorkspace : ClientProxyBase, ILanguageClientWorkspace
    {
        public LanguageClientWorkspace(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
