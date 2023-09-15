using System.Diagnostics;
using Newtonsoft.Json;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [JsonConverter(typeof(WorkspaceFolderOrUriConverter))]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public record WorkspaceFolderOrUri
    {
        public WorkspaceFolderOrUri(WorkspaceFolder value) => WorkspaceFolder = value;

        public WorkspaceFolderOrUri(DocumentUri value) => Uri = value;

        public WorkspaceFolder? WorkspaceFolder { get; }
        public bool HasWorkspaceFolder => WorkspaceFolder is { };
        public DocumentUri? Uri { get; }
        public bool HasUri => Uri is { };

        public static implicit operator WorkspaceFolderOrUri?(WorkspaceFolder value) =>
            value is null ? null : new WorkspaceFolderOrUri(value);

        public static implicit operator WorkspaceFolderOrUri?(DocumentUri value) =>
            value is null ? null : new WorkspaceFolderOrUri(value);

        private string DebuggerDisplay =>
            $"{( HasWorkspaceFolder ? WorkspaceFolder : HasUri ? Uri : string.Empty )}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
