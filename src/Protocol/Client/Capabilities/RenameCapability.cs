using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public class RenameCapability : DynamicCapability, ConnectedCapability<IRenameHandler>
    {
        /// <summary>
        /// Client supports testing for validity of rename operations
        /// before execution.
        /// </summary>
        [Optional]
        public bool PrepareSupport { get; set; }
    }
}
