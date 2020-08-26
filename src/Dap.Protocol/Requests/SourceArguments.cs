using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Source, Direction.ClientToServer)]
    public class SourceArguments : IRequest<SourceResponse>
    {
        /// <summary>
        /// Specifies the source content to load.Either source.path or source.sourceReference must be specified.
        /// </summary>
        [Optional]
        public Source? Source { get; set; }

        /// <summary>
        /// The reference to the source.This is the same as source.sourceReference.This is provided for backward compatibility since old backends do not understand the 'source' attribute.
        /// </summary>
        public long SourceReference { get; set; }
    }
}
