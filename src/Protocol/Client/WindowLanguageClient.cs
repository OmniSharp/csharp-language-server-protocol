using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    internal class WindowLanguageClient : LanguageProtocolProxy, IWindowLanguageClient
    {
        public WindowLanguageClient(
            IResponseRouter requestRouter, IResolverContext resolverContext, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings
        ) : base(requestRouter, resolverContext, progressManager, languageProtocolSettings)
        {
        }
    }
}
