using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public class LoadedSourceEvent : IRequest
    {

        /// <summary>
        /// The reason for the event.
        /// </summary>
        public LoadedSourceReason reason { get; set; }

        /// <summary>
        /// The new, changed, or removed source.
        /// </summary>
        public Source source { get; set; }
    }

}
