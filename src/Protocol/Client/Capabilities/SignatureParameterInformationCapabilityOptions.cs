using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SignatureParameterInformationCapabilityOptions
    {
        /// <summary>
        /// The client supports processing label offsets instead of a
        /// simple label string.
        /// </summary>
        [Optional]
        public bool LabelOffsetSupport { get; set; }
    }
}
