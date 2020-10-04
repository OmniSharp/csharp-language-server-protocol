using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Indicates which properties a client can resolve lazily on a completion
    /// item. Before version 3.16.0 only the predefined properties `documentation`
    /// and `details` could be resolved lazily.
    ///
    /// @since 3.16.0 - proposed state
    /// </summary>
    public class CompletionItemCapabilityResolveSupportOptions
    {
        /// <summary>
        /// The properties that a client can resolve lazily.
        /// </summary>
        public Container<string> Properties { get; set; } = new Container<string>();
    }
}
