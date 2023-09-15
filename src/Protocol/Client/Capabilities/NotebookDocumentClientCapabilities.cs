namespace OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;

/// <summary>
/// Capabilities specific to the notebook document support.
///
/// @since 3.17.0
/// </summary>
public class NotebookDocumentClientCapabilities
{
    /// <summary>
    /// Capabilities specific to notebook document synchronization
    ///
    /// @since 3.17.0
    /// </summary>
    public Supports<NotebookDocumentSyncClientCapabilities> Synchronization { get; set; }
}
