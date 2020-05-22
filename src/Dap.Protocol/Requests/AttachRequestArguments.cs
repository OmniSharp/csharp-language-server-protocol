using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using MediatR;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Attach, Direction.ClientToServer)]
    public class AttachRequestArguments : IRequest<AttachResponse>
    {
        /// <summary>
        /// Optional data from the previous, restarted session.
        /// The data is sent as the 'restart' attribute of the 'terminated' event.
        /// The client should leave the data intact.
        /// </summary>
        [Optional]
        public JToken Restart { get; set; }
    }

}
