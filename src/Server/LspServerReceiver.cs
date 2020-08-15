using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Server.Messages;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public class LspServerReceiver : Receiver, ILspServerReceiver
    {
        private readonly ILspHandlerTypeDescriptorProvider _handlerTypeDescriptorProvider;
        private bool _initialized;

        public LspServerReceiver(ILspHandlerTypeDescriptorProvider handlerTypeDescriptorProvider)
        {
            _handlerTypeDescriptorProvider = handlerTypeDescriptorProvider;
        }

        public override (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            if (_initialized) return base.GetRequests(container);

            var newResults = new List<Renor>();

            // Based on https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md#initialize-request
            var (results, hasResponse) = base.GetRequests(container);
            foreach (var item in results)
            {
                if (item.IsRequest && _handlerTypeDescriptorProvider.IsMethodName(item.Request.Method, typeof(ILanguageProtocolInitializeHandler)))
                {
                    newResults.Add(item);
                }
                else if (item.IsRequest)
                {
                    newResults.Add(new ServerNotInitialized(item.Request.Method));
                }
                else if (item.IsResponse)
                {
                    newResults.Add(item);
                }
                else if (item.IsNotification)
                {
                    // drop notifications
                    // newResults.Add(item);
                }
            }

            return ( newResults, hasResponse );
        }

        public void Initialized() => _initialized = true;

        public override bool ShouldFilterOutput(object value)
        {
            if (_initialized) return true;
            return value is OutgoingResponse ||
                   value is OutgoingNotification n && ( n.Params is LogMessageParams || n.Params is ShowMessageParams || n.Params is TelemetryEventParams ) ||
                   value is OutgoingRequest r && r.Params is ShowMessageRequestParams;
        }
    }
}
