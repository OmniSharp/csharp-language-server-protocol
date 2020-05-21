using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class InitializeResult
    {
        /// <summary>
        /// The capabilities the language server provides.
        /// </summary>
        public ServerCapabilities Capabilities { get; set; }

        /// <summary>
        ///  Information about the server.
        ///
        ///  @since 3.15.0
        /// </summary>
        [Optional]
        public ServerInfo ServerInfo { get; set; }
    }
}
