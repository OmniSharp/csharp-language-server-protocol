using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    internal class ClientLanguageServer : LanguageProtocolProxy, IClientLanguageServer
    {
        public ClientLanguageServer(
            IResponseRouter requestRouter, IServiceProvider serviceProvider, IProgressManager progressManager,
            ILanguageProtocolSettings languageProtocolSettings
        ) : base(requestRouter, serviceProvider, progressManager, languageProtocolSettings)
        {
        }
    }
}
