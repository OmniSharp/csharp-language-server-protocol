using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(JsonNumberEnumConverter))]
    public enum MessageType
    {
        /// <summary>
        ///  An error message.
        /// </summary>
        Error = 1,
        /// <summary>
        ///  A warning message.
        /// </summary>
        Warning = 2,
        /// <summary>
        ///  An information message.
        /// </summary>
        Info = 3,
        /// <summary>
        ///  A log message.
        /// </summary>
        Log = 4,
    }
}
