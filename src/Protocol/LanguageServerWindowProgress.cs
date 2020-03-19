using OmniSharp.Extensions.JsonRpc;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public class LanguageServerWindowProgress : ClientProxyBase, ILanguageServerWindowProgress
    {
        public LanguageServerWindowProgress(IResponseRouter responseRouter) : base(responseRouter) { }
    }
}
