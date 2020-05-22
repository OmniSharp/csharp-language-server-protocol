using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.SetExceptionBreakpoints, Direction.ClientToServer)]
    public class SetExceptionBreakpointsArguments : IRequest<SetExceptionBreakpointsResponse>
    {
        /// <summary>
        /// IDs of checked exception options.The set of IDs is returned via the 'exceptionBreakpointFilters' capability.
        /// </summary>
        public Container<string> Filters { get; set; }

        /// <summary>
        /// Configuration options for selected exceptions.
        /// </summary>
        [Optional] public Container<ExceptionOptions> ExceptionOptions { get; set; }
    }

}
