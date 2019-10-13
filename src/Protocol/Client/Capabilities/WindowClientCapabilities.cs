namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Window specific client capabilities.
    /// </summary>
    public class WindowClientCapabilities
    {
        /// <summary>
        /// Whether client supports handling progress notifications.
        /// </summary>
        public Supports<bool> WorkDoneProgress { get; set; }
    }
}
