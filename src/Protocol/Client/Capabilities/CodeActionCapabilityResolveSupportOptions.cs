using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Whether the client supports resolving additional code action
    /// properties via a separate `codeAction/resolve` request.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public class CodeActionCapabilityResolveSupportOptions
    {
        /// <summary>
        /// The properties that a client can resolve lazily.
        /// </summary>
        public Container<string> Properties { get; set; } = new Container<string>();
    }
}
