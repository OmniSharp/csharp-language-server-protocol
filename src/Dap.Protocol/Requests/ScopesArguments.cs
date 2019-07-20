using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class ScopesArguments : IRequest<ScopesResponse>
    {
        /// <summary>
        /// Retrieve the scopes for this stackframe.
        /// </summary>
        public long frameId { get; set; }
    }

}
