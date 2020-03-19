namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities
{
    public static class ClientCapabilitiesExtensions
    {
        /// <summary>
        /// Best attempt to determine if the hosting client supports a specific protocol version
        ///
        /// Capability are new as of 3.0, but the field existed before so it's possible
        ///     it could be passed as an empty object
        /// </summary>
        /// <param name="clientCapabilities">The capabilities.</param>
        /// <returns>ClientVersion.</returns>
        public static ClientVersion GetClientVersion(this ClientCapabilities clientCapabilities)
        {
            if (clientCapabilities == null || (clientCapabilities.TextDocument == null && clientCapabilities.Workspace == null))
                return ClientVersion.Lsp2;
            return ClientVersion.Lsp3;
        }
    }
}
