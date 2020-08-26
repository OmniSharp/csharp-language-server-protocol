using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class PublishDiagnosticsTagSupportCapabilityOptions
    {
        /// <summary>
        /// The tags supported by the client.
        /// </summary>
        public Container<DiagnosticTag> ValueSet { get; set; } = null!;
    }
}
