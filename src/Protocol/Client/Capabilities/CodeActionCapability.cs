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
        public CodeActionLiteralSupportOptions? CodeActionLiteralSupport { get; set; }

        /// <summary>
        /// Whether code action supports the `isPreferred` property.
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool IsPreferredSupport { get; set; }

        /// <summary>
        ///  Whether code action supports the `disabled` property.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public bool DisabledSupport { get; set; }

        /// <summary>
        /// Whether code action supports the `data` property which is
        /// preserved between a `textDocument/codeAction` and a `codeAction/resolve` request.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public bool DataSupport { get; set; }

        /// <summary>
        /// Whether the client supports resolving additional code action
        /// properties via a separate `codeAction/resolve` request.
        ///
        /// @since 3.16.0 - proposed state
        /// </summary>
        [Optional]
        public CodeActionCapabilityResolveSupportOptions? ResolveSupport { get; set; }
    }
}
