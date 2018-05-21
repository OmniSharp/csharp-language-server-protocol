using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class LanguageClientWindow : ClientProxyBase, ILanguageClientWindow
    {
        public LanguageClientWindow(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
