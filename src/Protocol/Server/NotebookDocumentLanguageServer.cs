
using DryIoc;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    internal class NotebookDocumentLanguageServer : LanguageProtocolProxy, INotebookDocumentLanguageServer
    {
        public NotebookDocumentLanguageServer(
            IResponseRouter requestRouter, IResolverContext resolverContext, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings
        ) : base(requestRouter, resolverContext, progressManager, languageProtocolSettings)
        {
        }
    }
}
