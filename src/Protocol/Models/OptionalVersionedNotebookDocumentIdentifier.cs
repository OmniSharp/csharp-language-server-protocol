﻿using System.Diagnostics;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models;

[DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
public record OptionalVersionedNotebookDocumentIdentifier : NotebookDocumentIdentifier
{
    /// <summary>
    /// The version number of this document.
    /// </summary>
    public int? Version { get; init; }

    private string DebuggerDisplay => $"{Uri}@({Version})";

    /// <inheritdoc />
    public override string ToString() => DebuggerDisplay;
}
