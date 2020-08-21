using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.CodeAction))]
    public class CodeActionCapability : DynamicCapability, ConnectedCapability<ICodeActionHandler>
    {
        /// <summary>
        /// The client support code action literals as a valid
        /// response of the `textDocument/codeAction` request.
        ///
        /// Since 3.8.0
        /// </summary>
        [Optional]
        public CodeActionLiteralSupportOptions CodeActionLiteralSupport { get; set; }

        /// <summary>
        /// Whether code action supports the `isPreferred` property.
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool IsPreferredSupport { get; set; }
    }
}
