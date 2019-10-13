namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    /// <summary>
    /// Window specific client capabilities.
    /// </summary>
    public class WindowCapability
    {
        /// <summary>
        /// Whether client supports handling progress notifications.
        /// </summary>
        public bool WorkDoneProgress { get; set; }
    }
}
