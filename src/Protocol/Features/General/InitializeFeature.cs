using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        public partial interface IInitializeParams<TClientCapabilities> : IWorkDoneProgressParams, IRequest<InitializeResult>
        {
            /// <summary>
            /// The process Id of the parent process that started
            /// the server. Is null if the process has not been started by another process.
            /// If the parent process is not alive then the server should exit (see exit notification) its process.
            /// </summary>
            long? ProcessId { get; init; }

            /// <summary>
            /// Information about the client
            ///
            /// @since 3.15.0
            /// </summary>
            [DisallowNull]
            ClientInfo? ClientInfo { get; init; }

            /// <summary>
            /// The rootPath of the workspace. Is null
            /// if no folder is open.
            ///
            /// @deprecated in favour of rootUri.
            /// </summary>
            [DisallowNull]
            string? RootPath { get; init; }

            /// <summary>
            /// The rootUri of the workspace. Is null if no
            /// folder is open. If both `rootPath` and `rootUri` are set
            /// `rootUri` wins.
            /// </summary>
            [DisallowNull]
            DocumentUri? RootUri { get; init; }

            /// <summary>
            /// User provided initialization options.
            /// </summary>
            [DisallowNull]
            object? InitializationOptions { get; init; }

            /// <summary>
            /// The capabilities provided by the client (editor or tool)
            /// </summary>
            [MaybeNull]
            TClientCapabilities Capabilities { get; init; }

            /// <summary>
            /// The initial trace setting. If omitted trace is disabled ('off').
            /// </summary>
            InitializeTrace Trace { get; init; }

            /// <summary>
            /// The workspace folders configured in the client when the server starts.
            /// This property is only available if the client supports workspace folders.
            /// It can be `null` if the client supports workspace folders but none are
            /// configured.
            ///
            /// Since 3.6.0
            /// </summary>
            [DisallowNull]
            Container<WorkspaceFolder>? WorkspaceFolders { get; init; }
        }

        [Method(GeneralNames.Initialize, Direction.ClientToServer)]
        public record InitializeParams : IInitializeParams<ClientCapabilities>
        {
            /// <summary>
            /// The process Id of the parent process that started
            /// the server. Is null if the process has not been started by another process.
            /// If the parent process is not alive then the server should exit (see exit notification) its process.
            /// </summary>
            public long? ProcessId { get; init; }

            /// <summary>
            /// Information about the client
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            [DisallowNull]
            public ClientInfo? ClientInfo { get; init; }

            /// <summary>
            /// The rootPath of the workspace. Is null
            /// if no folder is open.
            ///
            /// @deprecated in favour of rootUri.
            /// </summary>
            [Optional]
            [DisallowNull]
            public string? RootPath
            {
                get => RootUri?.GetFileSystemPath();
                init
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        RootUri = DocumentUri.FromFileSystemPath(value);
                    }
                }
            }

            /// <summary>
            /// The rootUri of the workspace. Is null if no
            /// folder is open. If both `rootPath` and `rootUri` are set
            /// `rootUri` wins.
            /// </summary>
            [DisallowNull]
            public DocumentUri? RootUri { get; init; }

            /// <summary>
            /// User provided initialization options.
            /// </summary>
            [DisallowNull]
            public object? InitializationOptions { get; init; }

            /// <summary>
            /// The capabilities provided by the client (editor or tool)
            /// </summary>
            [MaybeNull]
            public ClientCapabilities Capabilities { get; init; }

            /// <summary>
            /// The initial trace setting. If omitted trace is disabled ('off').
            /// </summary>
            [Optional]
            public InitializeTrace Trace { get; init; } = InitializeTrace.Off;

            /// <summary>
            /// The workspace folders configured in the client when the server starts.
            /// This property is only available if the client supports workspace folders.
            /// It can be `null` if the client supports workspace folders but none are
            /// configured.
            ///
            /// Since 3.6.0
            /// </summary>
            [MaybeNull]
            public Container<WorkspaceFolder>? WorkspaceFolders { get; init; }

            /// <inheritdoc />
            [Optional]
            [MaybeNull]
            public ProgressToken? WorkDoneToken { get; init; }

            /// <summary>
            /// The locale the client is currently showing the user interface
            /// in. This must not necessarily be the locale of the operating
            /// system.
            ///
            /// Uses IETF language tags as the value's syntax
            /// (See https://en.wikipedia.org/wiki/IETF_language_tag)
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public string? Locale { get; init; }

            public InitializeParams()
            {
            }

            internal InitializeParams(IInitializeParams<JObject> @params, ClientCapabilities clientCapabilities)
            {
                ProcessId = @params.ProcessId;
                Trace = @params.Trace;
                Capabilities = clientCapabilities;
                ClientInfo = @params.ClientInfo!;
                InitializationOptions = @params.InitializationOptions!;
                RootUri = @params.RootUri!;
                WorkspaceFolders = @params.WorkspaceFolders!;
                WorkDoneToken = @params.WorkDoneToken!;
            }
        }

        [Serial]
        [Method(GeneralNames.Initialize, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.General", Name = "LanguageProtocolInitialize")]
        [GenerateHandlerMethods(typeof(ILanguageServerRegistry))]
        [GenerateRequestMethods(typeof(ILanguageClient))]
        internal partial record
            InternalInitializeParams : IInitializeParams<JObject>, IRequest<InitializeResult> // This is required for generation to work correctly.
        {
            /// <summary>
            /// The process Id of the parent process that started
            /// the server. Is null if the process has not been started by another process.
            /// If the parent process is not alive then the server should exit (see exit notification) its process.
            /// </summary>
            public long? ProcessId { get; init; }

            /// <summary>
            /// Information about the client
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public ClientInfo? ClientInfo { get; init; }

            /// <summary>
            /// The rootPath of the workspace. Is null
            /// if no folder is open.
            ///
            /// @deprecated in favour of rootUri.
            /// </summary>
            [Optional]
            public string? RootPath
            {
                get => RootUri.GetFileSystemPath();
                init => RootUri = ( value == null ? null : DocumentUri.FromFileSystemPath(value) )!;
            }

            /// <summary>
            /// The rootUri of the workspace. Is null if no
            /// folder is open. If both `rootPath` and `rootUri` are set
            /// `rootUri` wins.
            /// </summary>
            public DocumentUri RootUri { get; init; } = null!;

            /// <summary>
            /// User provided initialization options.
            /// </summary>
            public object? InitializationOptions { get; init; }

            /// <summary>
            /// The capabilities provided by the client (editor or tool)
            /// </summary>
            public JObject Capabilities { get; init; } = null!;

            /// <summary>
            /// The initial trace setting. If omitted trace is disabled ('off').
            /// </summary>
            [Optional]
            public InitializeTrace Trace { get; init; } = InitializeTrace.Off;

            /// <summary>
            /// The workspace folders configured in the client when the server starts.
            /// This property is only available if the client supports workspace folders.
            /// It can be `null` if the client supports workspace folders but none are
            /// configured.
            ///
            /// Since 3.6.0
            /// </summary>
            public Container<WorkspaceFolder>? WorkspaceFolders { get; init; }

            /// <inheritdoc />
            [Optional]
            public ProgressToken? WorkDoneToken { get; init; }
        }

        public partial record InitializeResult
        {
            /// <summary>
            /// The capabilities the language server provides.
            /// </summary>
            public ServerCapabilities Capabilities { get; init; }

            /// <summary>
            /// Information about the server.
            ///
            /// @since 3.15.0
            /// </summary>
            [Optional]
            public ServerInfo? ServerInfo { get; init; }
        }

        public record InitializeError
        {
            /// <summary>
            /// Indicates whether the client should retry to send the
            /// initialize request after showing the message provided
            /// in the ResponseError.
            /// </summary>
            public bool Retry { get; init; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum InitializeTrace
        {
            [EnumMember(Value = "off")] Off,
            [EnumMember(Value = "messages")] Messages,
            [EnumMember(Value = "verbose")] Verbose
        }
    }
}
