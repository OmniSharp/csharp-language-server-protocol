using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

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
            var result = value switch {
                OutgoingResponse => true,
                OutgoingNotification n => n switch {
                    { Params: LogMessageParams }     => true,
                    { Params: ShowMessageParams }    => true,
                    { Params: TelemetryEventParams } => true,
                    _                                => false
                },
                OutgoingRequest { Params: ShowMessageRequestParams } => true,
                _                                                    => false
            };
            if (!result)
            {
                _logger.LogWarning("Tried to send request or notification before initialization was completed and will be sent later {@Request}", value);
            }

            return result;
        }
    }
}
