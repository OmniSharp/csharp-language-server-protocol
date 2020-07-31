using Newtonsoft.Json.Linq;
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
        [Optional]
        JObject Data { get; set; }
    }
}
