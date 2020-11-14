using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Shared;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    public class LspClientReceiver : Receiver, ILspClientReceiver
    {
        private readonly ILogger<LspClientReceiver> _logger;

        public LspClientReceiver(ILogger<LspClientReceiver> logger)
        {
            _logger = logger;
        }

        public override (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            if (_initialized) return base.GetRequests(container);

            var newResults = new List<Renor>();

            // Based on https://github.com/Microsoft/language-server-protocol/blob/master/protocol.md#initialize-request
            var (results, hasResponse) = base.GetRequests(container);
            foreach (var item in results)
            {
                switch (item)
                {
                    case { IsResponse: true }:
                    case { IsRequest: true, Request: { Method: WindowNames.ShowMessageRequest } }:
                    case { IsNotification: true, Notification: { Method: WindowNames.ShowMessage } }:
                    case { IsNotification: true, Notification: { Method: WindowNames.LogMessage } }:
                    case { IsNotification: true, Notification: { Method: WindowNames.TelemetryEvent } }:
                    case { IsNotification: true, Notification: { Method: WindowNames.WorkDoneProgressCancel } }:
                    case { IsNotification: true, Notification: { Method: WindowNames.WorkDoneProgressCreate } }:
                        newResults.Add(item);
                        break;
                    case { IsRequest: true, Request: { } }:
                        _logger.LogWarning("Unexpected request {Method} {@Request}", item.Request.Method, item.Request);
                        break;
                    case { IsNotification: true, Notification: { } }:
                        _logger.LogWarning("Unexpected notification {Method} {@Request}", item.Notification.Method, item.Notification);
                        break;
                    case { IsError: true, Error: { } }:
                        _logger.LogWarning("Unexpected error {Method} {@Request}", item.Error.Method, item.Error);
                        break;
                    default:
                        _logger.LogError("Unexpected Renor {@Renor}", item);
                        break;
                }
            }

            return ( newResults, hasResponse );
        }
    }
}
