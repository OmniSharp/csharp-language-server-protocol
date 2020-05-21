using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Server;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CodeActionCapability : DynamicCapability, ConnectedCapability<ICodeActionHandler>
    {
        /// <summary>
        /// The client support code action literals as a valid
        /// response of the `textDocument/codeAction` request.
        ///
        /// Since 3.8.0
        /// </summary>
        [Optional]
        public CodeActionLiteralSupportCapability CodeActionLiteralSupport { get; set; }

        /// <summary>
        /// Whether code action supports the `isPreferred` property.
        /// @since 3.15.0
        /// </summary>
        [Optional]
        public bool IsPreferredSupport { get; set; }
    }
}
