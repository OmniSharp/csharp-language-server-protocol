using System;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Flags]
    public enum WatchKind
    {
        /// <summary>
        /// Interested in create events.
        /// </summary>
        Create = 1,

        /// <summary>
        /// Interested in change events
        /// </summary>
        Change = 2,

        /// <summary>
        /// Interested in delete events
        /// </summary>
        Delete = 4,
    }
}
