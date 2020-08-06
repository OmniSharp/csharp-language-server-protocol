using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    internal class TextDocumentLanguageClient : ClientProxyBase, ITextDocumentLanguageClient
    {
        public TextDocumentLanguageClient(IResponseRouter requestRouter, IProgressManager progressManager, IClientWorkDoneManager clientWorkDoneManager, IRegistrationManager registrationManager, IWorkspaceFoldersManager workspaceFoldersManager, ILanguageProtocolSettings settings) : base(requestRouter, progressManager, clientWorkDoneManager, registrationManager, workspaceFoldersManager, settings)
        {
        }
    }
}
