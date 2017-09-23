using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Models
{
    /// <summary>
    ///  General paramters to to regsiter for a capability.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class Registration
    {
        /// <summary>
        ///  The id used to register the request. The id can be used to deregister
        ///  the request again.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        ///  The method / capability to register for.
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        ///  Options necessary for the registration.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object RegisterOptions { get; set; }
    }
}