using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record NotebookDocumentIdentifier
{
    public NotebookDocumentIdentifier()
    {
    }

    public NotebookDocumentIdentifier(DocumentUri uri)
    {
        Uri = uri;
    }

    /// <summary>
    /// The text document's URI.
    /// </summary>
    public DocumentUri Uri { get; init; } = null!;

    public static implicit operator NotebookDocumentIdentifier(DocumentUri uri)
    {
        return new NotebookDocumentIdentifier { Uri = uri };
    }

    public static implicit operator NotebookDocumentIdentifier(string uri)
    {
        return new NotebookDocumentIdentifier { Uri = uri };
    }

    // ReSharper disable once ConstantConditionalAccessQualifier
    private string DebuggerDisplay => Uri?.ToString() ?? string.Empty;

    /// <inheritdoc />
    public override string ToString()
    {
        return DebuggerDisplay;
    }
}
