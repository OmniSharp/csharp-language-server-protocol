using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    ///  Information about the server.
    ///
    ///  @since 3.15.0
    /// </summary>
    public class ServerInfo
    {
        /// <summary>
        ///  The name of the server as defined by the server.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///  The servers's version as defined by the server.
        /// </summary>
        [Optional]
        public string Version { get; set; }
    }
}
