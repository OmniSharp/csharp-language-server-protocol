using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    public class LanguageClientWindowProgress : ClientProxyBase, ILanguageClientWindowProgress
    {
        public LanguageClientWindowProgress(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
