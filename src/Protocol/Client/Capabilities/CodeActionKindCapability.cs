using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class CodeActionKindCapability
    {
        /// <summary>
        /// The code action kind values the client supports. When this
        /// property exists the client also guarantees that it will
        /// handle values outside its set gracefully and falls back
        /// to a default value when unknown.
        /// </summary>
        public Container<CodeActionKind> ValueSet { get; set; }
    }
}
