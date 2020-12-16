using System;
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
        public abstract record FileOperationParams<T>
            where T : FileOperationItem
        {
            /// <summary>
            /// An array of all files/folders deleted in this operation.
            /// </summary>
            public Container<T> Files { get; init; }
        }

        /// <summary>
        /// An array of all files/folders created in this operation.
        /// </summary>
        public abstract record FileOperationItem
        {
            /// <summary>
            /// A file:// URI for the location of the file/folder being created.
            /// </summary>
            public Uri Uri { get; init; }
        }

        /// <inheritdoc cref="FileOperationParams{T}"/>
        [Parallel]
        [Method(WorkspaceNames.DidCreateFiles, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DidCreateFileRegistrationOptions)), Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record DidCreateFileParams : FileOperationParams<FileCreate>, IRequest
        {
            public static implicit operator WillCreateFileParams(DidCreateFileParams @params) => new() { Files = @params.Files };
        }

        /// <inheritdoc cref="FileOperationParams{T}"/>
        [Parallel]
        [Method(WorkspaceNames.WillCreateFiles, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(WillCreateFileRegistrationOptions)), Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record WillCreateFileParams : FileOperationParams<FileCreate>, IRequest<WorkspaceEdit?>
        {
            public static implicit operator DidCreateFileParams(WillCreateFileParams @params) => new() { Files = @params.Files };
        }

        /// <inheritdoc cref="FileOperationItem"/>
        public partial record FileCreate : FileOperationItem
        {
        }
        /// <inheritdoc cref="FileOperationParams{T}"/>
        [Parallel]
        [Method(WorkspaceNames.DidRenameFiles, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DidRenameFileRegistrationOptions)), Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record DidRenameFileParams : FileOperationParams<FileRename>, IRequest
        {
            public static implicit operator WillRenameFileParams(DidRenameFileParams @params) => new() { Files = @params.Files };
        }

        /// <inheritdoc cref="FileOperationParams{T}"/>
        [Parallel]
        [Method(WorkspaceNames.WillRenameFiles, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(WillRenameFileRegistrationOptions)), Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record WillRenameFileParams : FileOperationParams<FileRename>, IRequest<WorkspaceEdit?>
        {
            public static implicit operator DidRenameFileParams(WillRenameFileParams @params) => new() { Files = @params.Files };
        }

        /// <inheritdoc cref="FileOperationItem"/>
        public partial record FileRename : FileOperationItem
        {
        }

        /// <inheritdoc cref="FileOperationParams{T}"/>
        [Parallel]
        [Method(WorkspaceNames.DidDeleteFiles, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(DidDeleteFileRegistrationOptions)), Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record DidDeleteFileParams : FileOperationParams<FileDelete>, IRequest
        {
            public static implicit operator WillDeleteFileParams(DidDeleteFileParams @params) => new() { Files = @params.Files };
        }

        /// <inheritdoc cref="FileOperationParams{T}"/>
        [Parallel]
        [Method(WorkspaceNames.WillDeleteFiles, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(WillDeleteFileRegistrationOptions)), Capability(typeof(FileOperationsWorkspaceClientCapabilities))]
        public partial record WillDeleteFileParams : FileOperationParams<FileDelete>, IRequest<WorkspaceEdit?>
        {
            public static implicit operator DidDeleteFileParams(WillDeleteFileParams @params) => new() { Files = @params.Files };
        }

        /// <inheritdoc cref="FileOperationItem"/>
        public partial record FileDelete : FileOperationItem
        {
        }

        /// <inheritdoc cref="FileOperationRegistrationOptions"/>
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations), nameof(FileOperationsWorkspaceServerCapabilities.WillCreate)
        )]
        [RegistrationName(WorkspaceNames.WillCreateFiles)]
        public partial class WillCreateFileRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="FileOperationRegistrationOptions"/>
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations), nameof(FileOperationsWorkspaceServerCapabilities.DidCreate)
        )]
        [RegistrationName(WorkspaceNames.DidCreateFiles)]
        public partial class DidCreateFileRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="FileOperationRegistrationOptions"/>
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations), nameof(FileOperationsWorkspaceServerCapabilities.WillRename)
        )]
        [RegistrationName(WorkspaceNames.WillRenameFiles)]
        public partial class WillRenameFileRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="FileOperationRegistrationOptions"/>
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations), nameof(FileOperationsWorkspaceServerCapabilities.DidRename)
        )]
        [RegistrationName(WorkspaceNames.DidRenameFiles)]
        public partial class DidRenameFileRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="FileOperationRegistrationOptions"/>
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations), nameof(FileOperationsWorkspaceServerCapabilities.WillDelete)
        )]
        [RegistrationName(WorkspaceNames.WillDeleteFiles)]
        public partial class WillDeleteFileRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        /// <inheritdoc cref="FileOperationRegistrationOptions"/>
        [GenerateRegistrationOptions(
            nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.FileOperations), nameof(FileOperationsWorkspaceServerCapabilities.DidDelete)
        )]
        [RegistrationName(WorkspaceNames.DidDeleteFiles)]
        public partial class DidDeleteFileRegistrationOptions : IFileOperationRegistrationOptions
        {
            /// <summary>
            /// The actual filters.
            /// </summary>
            public Container<FileOperationFilter> Filters { get; set; } = new Container<FileOperationFilter>();
        }

        ///<summary>
        /// The options to register for file operations.
        ///
        /// @since 3.16.0
        ///</summary>
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
            ///   and JavaScript files)
            /// - `[]` to declare a range of characters to match in a path segment
            ///   (e.g., `example.[0-9]` to match on `example.0`, `example.1`, …)
            /// - `[!...]` to negate a range of characters to match in a path segment
            ///   (e.g., `example.[!0-9]` to match on `example.a`, `example.b`, but
            ///   not `example.0`)
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
        public class FileOperationsWorkspaceServerCapabilities : DynamicCapability
        {
            /// <summary>
            /// The client has support for sending didCreateFiles notifications.
            /// </summary>
            [Optional]
            public DidCreateFileRegistrationOptions.StaticOptions? DidCreate { get; set; }

            /// <summary>
            /// The client has support for sending willCreateFiles requests.
            /// </summary>
            [Optional]
            public WillCreateFileRegistrationOptions.StaticOptions? WillCreate { get; set; }

            /// <summary>
            /// The client has support for sending didRenameFiles notifications.
            /// </summary>
            [Optional]
            public DidRenameFileRegistrationOptions.StaticOptions? DidRename { get; set; }

            /// <summary>
            /// The client has support for sending willRenameFiles requests.
            /// </summary>
            [Optional]
            public WillRenameFileRegistrationOptions.StaticOptions? WillRename { get; set; }

            /// <summary>
            /// The client has support for sending didDeleteFiles notifications.
            /// </summary>
            [Optional]
            public DidDeleteFileRegistrationOptions.StaticOptions? DidDelete { get; set; }

            /// <summary>
            /// The client has support for sending willDeleteFiles requests.
            /// </summary>
            [Optional]
            public WillDeleteFileRegistrationOptions.StaticOptions? WillDelete { get; set; }
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
