using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

/// <summary>
/// A versioned notebook document identifier.
///
/// @since 3.17.0
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public partial record VersionedNotebookDocumentIdentifier {
    /// <summary>
    /// The version number of this notebook document.
    /// </summary>
    public int Version {get; set;}
    /// <summary>
    /// The notebook document's uri.
    /// </summary>
    public DocumentUri Uri {get; set;}

    private string DebuggerDisplay => $"{Uri}@({Version})";

    /// <inheritdoc />
    public override string ToString() => DebuggerDisplay;
}
