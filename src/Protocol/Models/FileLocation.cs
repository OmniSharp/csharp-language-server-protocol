using System.Diagnostics;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
[GenerateContainer]
public record FileLocation
{
    /// <summary>
    /// The uri of the document
    /// </summary>
    public DocumentUri Uri { get; init; } = null!;

    private string DebuggerDisplay => $"{{{Uri}}}";

    /// <inheritdoc />
    public override string ToString()
    {
        return DebuggerDisplay;
    }
}