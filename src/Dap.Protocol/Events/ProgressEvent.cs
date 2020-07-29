using OmniSharp.Extensions.DebugAdapter.Protocol.Models;
using OmniSharp.Extensions.DebugAdapter.Protocol.Serialization;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.Events
{
    public abstract class ProgressEvent
    {
        /// <summary>
        ///  The ID that was introduced in the initial 'progressStart' event.
        /// </summary>
        public ProgressToken ProgressId { get; set; }

        /// <summary>
        ///  Optional, more detailed progress message. If omitted, the previous message (if any) is used.
        /// </summary>
        [Optional]
        public string Message { get; set; }
    }
}
