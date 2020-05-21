using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class SignatureHelpCapability : DynamicCapability, ConnectedCapability<ISignatureHelpHandler>
    {
        /// <summary>
        /// The client supports the following `SignatureInformation`
        /// specific properties.
        /// </summary>
        [Optional]
        public SignatureInformationCapability SignatureInformation { get; set; }

        /// <summary>
        /// The client supports to send additional context information for a
        /// `textDocument/signatureHelp` request. A client that opts into
        /// contextSupport will also support the `retriggerCharacters` on
        /// `SignatureHelpOptions`.
        ///
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool ContextSupport { get; set; }
    }
}
