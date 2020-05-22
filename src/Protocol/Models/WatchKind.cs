using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Flags]
    [JsonConverter(typeof(NumberEnumConverter))]
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
