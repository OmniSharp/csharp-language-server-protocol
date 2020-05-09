using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities
{
    /// <summary>
    ///  Save options.
    /// </summary>
    public class SaveOptions
    {
        /// <summary>
        ///  The client is supposed to include the content on save.
        /// </summary>
        [Optional]
        public bool IncludeText { get; set; }
    }
}
