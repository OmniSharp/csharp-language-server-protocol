using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  The file event type.
    /// </summary>
    [JsonConverter(typeof(JsonNumberEnumConverter))]
    public enum FileChangeType
    {
        /// <summary>
        ///  The file got created.
        /// </summary>
        Created = 1,
        /// <summary>
        ///  The file got changed.
        /// </summary>
        Changed = 2,
        /// <summary>
        ///  The file got deleted.
        /// </summary>
        Deleted = 3,
    }
}
