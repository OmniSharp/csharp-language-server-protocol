using MediatR;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Requests
{
    [Method(RequestNames.Completions, Direction.ClientToServer)]
    public class CompletionsArguments : IRequest<CompletionsResponse>
    {
        /// <summary>
        /// Returns completions in the scope of this stack frame. If not specified, the completions are returned for the global scope.
        /// </summary>
        [Optional]
        public long? FrameId { get; set; }

        /// <summary>
        /// One or more source lines.Typically this is the text a user has typed into the debug console before he asked for completion.
        /// </summary>
        public string Text { get; set; } = null!;

        /// <summary>
        /// The character position for which to determine the completion proposals.
        /// </summary>
        public long Column { get; set; }

        /// <summary>
        /// An optional line for which to determine the completion proposals.If missing the first line of the text is assumed.
        /// </summary>
        [Optional]
        public long? Line { get; set; }
    }
}
