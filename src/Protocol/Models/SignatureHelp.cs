using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Signature help represents the signature of something
    /// callable. There can be multiple signature but only one
    /// active and only one active parameter.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class SignatureHelp
    {
        /// <summary>
        /// One or more signatures.
        /// </summary>
        public Container<SignatureInformation> Signatures { get; set; }

        /// <summary>
        /// The active signature.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long ActiveSignature { get; set; }

        /// <summary>
        /// The active parameter of the active signature.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long ActiveParameter { get; set; }
    }
}