using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    class LspClientOutputFilter : IOutputFilter
    {
        private readonly ILogger<LspClientOutputFilter> _logger;

        public LspClientOutputFilter(ILogger<LspClientOutputFilter> logger)
        {
            _logger = logger;
        }

        public bool ShouldOutput(object value)
        {
            var result = value switch {
                OutgoingResponse                                   => true,
                OutgoingRequest { Params: InitializeParams }       => true,
                OutgoingNotification { Params: InitializedParams } => true,
                _                                                  => false
            };
            if (!result)
            {
                _logger.LogWarning("Tried to send request or notification before initialization was completed {@Request}", value);
            }

            return result;
        }
    }
}
