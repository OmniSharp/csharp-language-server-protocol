using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Common interface for types that support resolution.
    /// </summary>
    public interface ICanBeResolved
    {
        /// <summary>
        /// A data entry field that is preserved for resolve requests
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        JsonElement Data { get; set; }
    }
}
