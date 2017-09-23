using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class CancelParams
    {
        /// <summary>
        /// The request id to cancel.
        /// </summary>
        public object Id { get; set; }
    }
}