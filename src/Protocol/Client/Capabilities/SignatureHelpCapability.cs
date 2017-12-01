using Newtonsoft.Json;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SignatureHelpCapability : DynamicCapability, ConnectedCapability<ISignatureHelpHandler>
    {
        /// <summary>
        /// The client supports the following `SignatureInformation`
        /// specific properties.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SignatureInformationCapability SignatureInformation { get; set; }
    }
}
