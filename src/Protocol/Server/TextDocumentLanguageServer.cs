using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    internal class TextDocumentLanguageServer : LanguageProtocolProxy, ITextDocumentLanguageServer
    {
        public TextDocumentLanguageServer(
            IResponseRouter requestRouter, IResolverContext resolverContext, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings
        ) : base(requestRouter, resolverContext, progressManager, languageProtocolSettings)
        {
        }
    }
}
