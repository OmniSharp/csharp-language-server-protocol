using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SignatureParameterInformationCapability
    {
        /// <summary>
        /// The client supports processing label offsets instead of a
        /// simple label string.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenNull)]
        public bool LabelOffsetSupport { get; set; }
    }
}
