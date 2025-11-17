using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        /// <summary>
        /// The parameters sent in notifications/requests for user-initiated creation
        /// of files.
        ///
        /// @since 3.16.0
        /// </summary>
        public abstract record FileOperationsParams<T>
            where T : FileOperationItem
        {
            /// <summary>
            /// An array of all files/folders deleted in this operation.
            /// </summary>
            public Container<T> Files { get; init; } = null!;
        }

        /// <summary>
        /// An array of all files/folders created in this operation.
        /// </summary>
        public abstract record FileOperationItem
        {
            /// <summary>
            /// A file:// URI for the location of the file/folder being created.
            /// </summary>
            public Uri Uri { get; init; } = null!;
        }

        /// <inheritdoc cref="FileOperationsParams{T}" />
        [Parallel]
        [Method(WorkspaceNames.DidCreateFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DidCreateFilesRegistrationOptions))]
        [Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record DidCreateFilesParams : FileOperationsParams<FileCreate>, IRequest<Unit>
        {
            public static implicit operator WillCreateFilesParams(DidCreateFilesParams @params)
            {
                return new() { Files = @params.Files };
            }
        }

        /// <inheritdoc cref="FileOperationsParams{T}" />
        [Parallel]
        [Method(WorkspaceNames.WillCreateFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(WillCreateFilesRegistrationOptions))]
        [Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record WillCreateFilesParams : FileOperationsParams<FileCreate>, IRequest<WorkspaceEdit?>
        {
            public static implicit operator DidCreateFilesParams(WillCreateFilesParams @params)
            {
                return new() { Files = @params.Files };
            }
        }

        /// <inheritdoc cref="FileOperationItem" />
        public partial record FileCreate : FileOperationItem;

        /// <inheritdoc cref="FileOperationsParams{T}" />
        [Parallel]
        [Method(WorkspaceNames.DidRenameFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DidRenameFilesRegistrationOptions))]
        [Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record DidRenameFilesParams : RenameFilesOperationParams, IRequest<Unit>
        {
            public static implicit operator WillRenameFilesParams(DidRenameFilesParams @params)
            {
                return new() { Files = @params.Files };
            }
        }

        /// <inheritdoc cref="FileOperationsParams{T}" />
        [Parallel]
        [Method(WorkspaceNames.WillRenameFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(WillRenameFilesRegistrationOptions))]
        [Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record WillRenameFilesParams : RenameFilesOperationParams, IRequest<WorkspaceEdit?>
        {
            public static implicit operator DidRenameFilesParams(WillRenameFilesParams @params)
            {
                return new() { Files = @params.Files };
            }
        }

        /// <summary>
        /// The parameters sent in notifications/requests for user-initiated renames
        /// of files.
        ///
        /// @since 3.16.0
        /// </summary>
        public abstract record RenameFilesOperationParams
        {
            /// <summary>
            /// An array of all files/folders renamed in this operation.
            /// When a folder is renamed, only the folder will be included, and not its children.
            /// </summary>
            public Container<FileRename> Files { get; init; } = null!;
        }

        /// <summary>
        /// Represents information on a file/folder rename.
        /// </summary>
        public partial record FileRename
        {
            /// <summary>
            /// A file:// URI for the original location of the file/folder being renamed.
            /// </summary>
            public Uri OldUri { get; init; } = null!;

            /// <summary>
            /// A file:// URI for the new location of the file/folder being renamed.
            /// </summary>
            public Uri NewUri { get; init; } = null!;
        }

        /// <inheritdoc cref="FileOperationsParams{T}" />
        [Parallel]
        [Method(WorkspaceNames.DidDeleteFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(DidDeleteFilesRegistrationOptions))]
        [Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record DidDeleteFilesParams : FileOperationsParams<FileDelete>, IRequest<Unit>
        {
            public static implicit operator WillDeleteFilesParams(DidDeleteFilesParams @params)
            {
                return new() { Files = @params.Files };
            }
        }

        /// <inheritdoc cref="FileOperationsParams{T}" />
        [Parallel]
        [Method(WorkspaceNames.WillDeleteFiles, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(WillDeleteFilesRegistrationOptions))]
        [Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record WillDeleteFilesParams : FileOperationsParams<FileDelete>, IRequest<WorkspaceEdit?>
        {
            public static implicit operator DidDeleteFilesParams(WillDeleteFilesParams @params)
            {
                return new() { Files = @params.Files };
            }
        }

        /// <inheritdoc cref="FileOperationItem" />
        public partial record FileDelete : FileOperationItem;

        /// <inheritdoc cref="IFileOperationRegistrationOptions" />
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations),
            nameof(FileOperationsWorkspaceServerCapabilities.WillCreate)
        )]
        [RegistrationName(WorkspaceNames.WillCreateFiles)]
        public partial class WillCreateFilesRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="IFileOperationRegistrationOptions" />
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations),
            nameof(FileOperationsWorkspaceServerCapabilities.DidCreate)
        )]
        [RegistrationName(WorkspaceNames.DidCreateFiles)]
        public partial class DidCreateFilesRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="IFileOperationRegistrationOptions" />
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations),
            nameof(FileOperationsWorkspaceServerCapabilities.WillRename)
        )]
        [RegistrationName(WorkspaceNames.WillRenameFiles)]
        public partial class WillRenameFilesRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="IFileOperationRegistrationOptions" />
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations),
            nameof(FileOperationsWorkspaceServerCapabilities.DidRename)
        )]
        [RegistrationName(WorkspaceNames.DidRenameFiles)]
        public partial class DidRenameFilesRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="IFileOperationRegistrationOptions" />
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations),
            nameof(FileOperationsWorkspaceServerCapabilities.WillDelete)
        )]
        [RegistrationName(WorkspaceNames.WillDeleteFiles)]
        public partial class WillDeleteFilesRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="IFileOperationRegistrationOptions" />
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations),
            nameof(FileOperationsWorkspaceServerCapabilities.DidDelete)
        )]
        [RegistrationName(WorkspaceNames.DidDeleteFiles)]
        public partial class DidDeleteFilesRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <summary>
        /// The options to register for file operations.
        /// 
        /// @since 3.16.0
        /// </summary>
        public interface IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; }
        }

        /// <summary>
        /// A pattern kind describing if a glob pattern matches a file a folder or
        /// both.
        ///
        /// @since 3.16.0
        /// </summary>
        [StringEnum]
        public readonly partial struct FileOperationPatternKind
        {
            public static FileOperationPatternKind File { get; } = new FileOperationPatternKind("file");
            public static FileOperationPatternKind Folder { get; } = new FileOperationPatternKind("folder");
        }

        /// <summary>
        /// Additional options used during matching.
        /// </summary>
        public record FileOperationPatternOptions
        {
            /// <summary>
            /// The pattern should be matched ignoring casing.
            /// </summary>
            [Optional]
            public bool IgnoreCase { get; init; }
        }

        /// <summary>
        /// A pattern to describe in which file operation requests or notifications
        /// the server is interested in.
        ///
        /// @since 3.16.0
        /// </summary>
        public record FileOperationPattern
        {
            /// <summary>
            /// The glob pattern to match. Glob patterns can have the following syntax:
            /// - `*` to match one or more characters in a path segment
            /// - `?` to match on one character in a path segment
            /// - `**` to match any number of path segments, including none
            /// - `{}` to group conditions (e.g. `**​/*.{ts,js}` matches all TypeScript
            /// and JavaScript files)
            /// - `[]` to declare a range of characters to match in a path segment
            /// (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
            /// - `[!...]` to negate a range of characters to match in a path segment
            /// (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
            /// not `example.0`)
            /// </summary>
            public string Glob { get; init; }

            /// <summary>
            /// Whether to match files or folders with this pattern.
            ///
            /// Matches both if undefined.
            /// </summary>
            [Optional]
            public FileOperationPatternKind Matches { get; init; }

            /// <summary>
            /// Additional options used during matching.
            /// </summary>
            [Optional]
            public FileOperationPatternOptions? Options { get; init; }
        }

        /// <summary>
        /// A filter to describe in which file operation requests or notifications
        /// the server is interested in.
        ///
        /// @since 3.16.0
        /// </summary>
        public record FileOperationFilter
        {
            /// <summary>
            /// A Uri like `file` or `untitled`.
            /// </summary>
            [Optional]
            public string? Scheme { get; init; }

            /// <summary>
            /// The actual file operation pattern.
            /// </summary>
            [Optional]
            public FileOperationPattern? Pattern { get; init; }
        }
    }

    namespace Server.Capabilities
    {
        public class FileOperationsWorkspaceServerCapabilities
        {
            /// <summary>
            /// The client has support for sending didCreateFiles notifications.
            /// </summary>
            [Optional]
            public DidCreateFilesRegistrationOptions.StaticOptions? DidCreate { get; set; }

            /// <summary>
            /// The client has support for sending willCreateFiles requests.
            /// </summary>
            [Optional]
            public WillCreateFilesRegistrationOptions.StaticOptions? WillCreate { get; set; }

            /// <summary>
            /// The client has support for sending didRenameFiles notifications.
            /// </summary>
            [Optional]
            public DidRenameFilesRegistrationOptions.StaticOptions? DidRename { get; set; }

            /// <summary>
            /// The client has support for sending willRenameFiles requests.
            /// </summary>
            [Optional]
            public WillRenameFilesRegistrationOptions.StaticOptions? WillRename { get; set; }

            /// <summary>
            /// The client has support for sending didDeleteFiles notifications.
            /// </summary>
            [Optional]
            public DidDeleteFilesRegistrationOptions.StaticOptions? DidDelete { get; set; }

            /// <summary>
            /// The client has support for sending willDeleteFiles requests.
            /// </summary>
            [Optional]
            public WillDeleteFilesRegistrationOptions.StaticOptions? WillDelete { get; set; }
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.FileOperations))]
        public class FileOperationsWorkspaceClientCapabilities : DynamicCapability
        {
            /// <summary>
            /// The client has support for sending didCreateFiles notifications.
            /// </summary>
            [Optional]
            public bool DidCreate { get; set; }

            /// <summary>
            /// The client has support for sending willCreateFiles requests.
            /// </summary>
            [Optional]
            public bool WillCreate { get; set; }

            /// <summary>
            /// The client has support for sending didRenameFiles notifications.
            /// </summary>
            [Optional]
            public bool DidRename { get; set; }

            /// <summary>
            /// The client has support for sending willRenameFiles requests.
            /// </summary>
            [Optional]
            public bool WillRename { get; set; }

            /// <summary>
            /// The client has support for sending didDeleteFiles notifications.
            /// </summary>
            [Optional]
            public bool DidDelete { get; set; }

            /// <summary>
            /// The client has support for sending willDeleteFiles requests.
            /// </summary>
            [Optional]
            public bool WillDelete { get; set; }
        }
    }
}
