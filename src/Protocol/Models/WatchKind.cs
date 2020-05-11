using System;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [Flags]
    [JsonConverter(typeof(JsonNumberEnumConverter))]
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
