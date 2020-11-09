using System.Collections.Generic;
using Microsoft.Extensions.Logging;
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
    class LspServerOutputFilter : IOutputFilter
    {
        private readonly ILogger<LspServerOutputFilter> _logger;

        public LspServerOutputFilter(ILogger<LspServerOutputFilter> logger)
        {
            _logger = logger;
        }

        public bool ShouldOutput(object value)
        {
            var result = value is OutgoingResponse ||
                         value is OutgoingNotification n && ( n.Params is LogMessageParams || n.Params is ShowMessageParams || n.Params is TelemetryEventParams ) ||
                         value is OutgoingRequest { Params: ShowMessageRequestParams };
            if (!result)
            {
                _logger.LogWarning("Tried to send request or notification before initialization was completed {@Request}", value);
            }

            return result;
        }
    }
    public class LspServerReceiver : Receiver, ILspServerReceiver
    {
        private readonly ILspHandlerTypeDescriptorProvider _handlerTypeDescriptorProvider;

        public LspServerReceiver(ILspHandlerTypeDescriptorProvider handlerTypeDescriptorProvider, IEnumerable<IOutputFilter> outputFilters) : base(outputFilters)
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
                if (item.IsRequest && _handlerTypeDescriptorProvider.IsMethodName(item.Request!.Method, typeof(ILanguageProtocolInitializeHandler)))
                {
                    newResults.Add(item);
                }
                else if (item.IsRequest)
                {
                    newResults.Add(new ServerNotInitialized(item.Request!.Method));
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
    }
}
