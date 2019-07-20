using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class GotoTargetsArguments : IRequest<GotoTargetsResponse>
    {
        /// <summary>
        /// The source location for which the goto targets are determined.
        /// </summary>
        public Source source { get; set; }

        /// <summary>
        /// The line location for which the goto targets are determined.
        /// </summary>
        public long line { get; set; }

        /// <summary>
        /// An optional column location for which the goto targets are determined.
        /// </summary>
        [Optional] public long? column { get; set; }
    }

}
