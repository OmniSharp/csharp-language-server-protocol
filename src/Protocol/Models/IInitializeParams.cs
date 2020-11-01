using System.Diagnostics.CodeAnalysis;
using MediatR;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public interface IInitializeParams<TClientCapabilities> : IWorkDoneProgressParams, IRequest<InitializeResult>
    {
        /// <summary>
        /// The process Id of the parent process that started
        /// the server. Is null if the process has not been started by another process.
        /// If the parent process is not alive then the server should exit (see exit notification) its process.
        /// </summary>
        long? ProcessId { get; set; }

        /// <summary>
        /// Information about the client
        ///
        /// @since 3.15.0
        /// </summary>
        [DisallowNull] ClientInfo? ClientInfo { get; set; }

        /// <summary>
        /// The rootPath of the workspace. Is null
        /// if no folder is open.
        ///
        /// @deprecated in favour of rootUri.
        /// </summary>
        [DisallowNull] string? RootPath { get; set; }

        /// <summary>
        /// The rootUri of the workspace. Is null if no
        /// folder is open. If both `rootPath` and `rootUri` are set
        /// `rootUri` wins.
        /// </summary>
        [DisallowNull] DocumentUri? RootUri { get; set; }

        /// <summary>
        /// User provided initialization options.
        /// </summary>
        [DisallowNull] object? InitializationOptions { get; set; }

        /// <summary>
        /// The capabilities provided by the client (editor or tool)
        /// </summary>
        [MaybeNull] TClientCapabilities Capabilities { get; set; }

        /// <summary>
        /// The initial trace setting. If omitted trace is disabled ('off').
        /// </summary>
        InitializeTrace Trace { get; set; }

        /// <summary>
        /// The workspace folders configured in the client when the server starts.
        /// This property is only available if the client supports workspace folders.
        /// It can be `null` if the client supports workspace folders but none are
        /// configured.
        ///
        /// Since 3.6.0
        /// </summary>
        [DisallowNull] Container<WorkspaceFolder>? WorkspaceFolders { get; set; }
    }
}
