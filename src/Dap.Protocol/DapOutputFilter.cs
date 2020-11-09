using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.DebugAdapter.Protocol.Events;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    class DapOutputFilter : IOutputFilter
    {
        private readonly ILogger<DapOutputFilter> _logger;
        public DapOutputFilter(ILogger<DapOutputFilter> logger)
        {
            _logger = logger;
        }
        
        public bool ShouldOutput(object value)
        {
            var result = value is OutgoingResponse ||
                         value is OutgoingNotification { Params: InitializedEvent } ||
                         value is OutgoingRequest { Params: InitializeRequestArguments };

            if (!result)
            {
                _logger.LogWarning("Tried to send request or notification before initialization was completed {@Request}", value);
            }

            return result;
        }
    }
}
