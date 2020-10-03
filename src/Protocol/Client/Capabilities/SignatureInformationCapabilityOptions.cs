using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SignatureInformationCapabilityOptions
    {
        /// <summary>
        /// Client supports the follow content formats for the content property. The order describes the preferred format of the client.
        /// </summary>
        [Optional]
        public Container<MarkupKind>? DocumentationFormat { get; set; }

        [Optional] public SignatureParameterInformationCapabilityOptions? ParameterInformation { get; set; }

        /// <summary>
        /// The client support the `activeParameter` property on `SignatureInformation`
        /// literal.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public bool ActiveParameterSupport { get; set; }
    }
}
