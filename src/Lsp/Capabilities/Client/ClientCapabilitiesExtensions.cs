namespace OmniSharp.Extensions.LanguageServerProtocol.Capabilities.Client
{
    public static class ClientCapabilitiesExtensions
    {
        /// <summary>
        /// Best attempt to determine if the hosting client supports a specific protocol version
        ///
        /// ClientCapabilities are new as of 3.0, but the field existsed before so it's possible
        ///     it could be passed as an empty object
        /// </summary>
        /// <param name="capabilities">The capabilities.</param>
        /// <returns>ClientVersion.</returns>
        public static ClientVersion GetClientVersion(this ClientCapabilities capabilities)
        {
            if (capabilities == null || (capabilities.TextDocument == null && capabilities.Workspace == null))
                return ClientVersion.Lsp2;
            return ClientVersion.Lsp3;
        }
    }
}