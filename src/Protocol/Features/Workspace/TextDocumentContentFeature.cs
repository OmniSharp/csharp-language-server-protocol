using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(WorkspaceNames.TextDocumentContent, Direction.ClientToServer)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageClient), typeof(ILanguageClient))]
        [RegistrationOptions(typeof(TextDocumentContentRegistrationOptions))]
        [Capability(typeof(TextDocumentContentClientCapabilities))]
        public partial record TextDocumentContentParams : IRequest<TextDocumentContentResult>
        {
            /// <summary>
            /// The URI of the text document.
            /// </summary>
            public DocumentUri Uri { get; init; }
        }

        /// <summary>
        /// Result of the `workspace/textDocumentContent` request.
        ///
        /// @since 3.18.0
        /// </summary>
        public partial record TextDocumentContentResult
        {
            /// <summary>
            /// The text content of the text document.
            /// </summary>
            public string Text { get; init; } = null!;
        }

        [Parallel]
        [Method(WorkspaceNames.TextDocumentContentRefresh, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace")]
        [GenerateHandlerMethods]
        [GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        public partial record TextDocumentContentRefreshParams : IRequest<Unit>
        {
            /// <summary>
            /// The URI of the text document to refresh.
            /// </summary>
            public DocumentUri Uri { get; init; }
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.Workspace), nameof(WorkspaceServerCapabilities.TextDocumentContent))]
        [RegistrationName(WorkspaceNames.TextDocumentContent)]
        public partial class TextDocumentContentRegistrationOptions : IStaticRegistrationOptions
        {
            /// <summary>
            /// The schemes for which the server provides content.
            /// </summary>
            public Container<string> Schemes { get; set; } = new();
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.TextDocumentContent))]
        public partial class TextDocumentContentClientCapabilities : DynamicCapability
        {
        }
    }
}
