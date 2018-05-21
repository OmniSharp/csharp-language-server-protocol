using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class LanguageServerWorkspace : ClientProxyBase, ILanguageServerWorkspace
    {
        public LanguageServerWorkspace(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
