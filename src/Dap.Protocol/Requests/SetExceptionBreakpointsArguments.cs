using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
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
