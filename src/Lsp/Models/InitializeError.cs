using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServerProtocol.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class InitializeError
    {
        /// <summary>
        /// Indicates whether the client should retry to send the
        /// initilize request after showing the message provided
        /// in the ResponseError.
        /// </summary>
        public bool Retry { get; set; }
    }
}