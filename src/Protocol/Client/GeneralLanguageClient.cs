using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client
{
    internal class GeneralLanguageClient : ClientProxyBase, IGeneralLanguageClient
    {
        public GeneralLanguageClient(IResponseRouter requestRouter, IProgressManager progressManager, Lazy<IClientWorkDoneManager> clientWorkDoneManager, Lazy<IRegistrationManager> registrationManager, Lazy<IWorkspaceFoldersManager> workspaceFoldersManager, ILanguageProtocolSettings settings) : base(requestRouter, progressManager, clientWorkDoneManager, registrationManager, workspaceFoldersManager, settings)
        {
        }
    }
}
