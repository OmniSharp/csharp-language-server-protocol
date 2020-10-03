using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LspClientReceiver : Receiver, ILspClientReceiver
    {
        private readonly ILspHandlerTypeDescriptorProvider _handlerTypeDescriptorProvider;
        private bool _initialized;

        public LspClientReceiver(ILspHandlerTypeDescriptorProvider handlerTypeDescriptorProvider)
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
                if (item.IsRequest && _handlerTypeDescriptorProvider.IsMethodName(item.Request!.Method, typeof(IShowMessageRequestHandler)))
                {
                    newResults.Add(item);
                }
                else if (item.IsResponse)
                {
                    newResults.Add(item);
                }
                else if (item.IsNotification &&
                         _handlerTypeDescriptorProvider.IsMethodName(
                             item.Notification!.Method,
                             typeof(IShowMessageHandler),
                             typeof(ILogMessageHandler),
                             typeof(ITelemetryEventHandler)
                         )
                )
                {
                    newResults.Add(item);
                }
            }

            return ( newResults, hasResponse );
        }

        public void Initialized() => _initialized = true;

        public override bool ShouldFilterOutput(object value)
        {
            if (_initialized) return true;
            return value is OutgoingResponse || value is OutgoingRequest v && v.Params is InitializeParams;
        }
    }
}
