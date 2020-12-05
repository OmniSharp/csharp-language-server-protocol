using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using DryIoc;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Method(TextDocumentNames.CodeLens, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(CodeLensRegistrationOptions)), Capability(typeof(CodeLensCapability)), Resolver(typeof(CodeLens))]
        public partial record CodeLensParams : ITextDocumentIdentifierParams, IWorkDoneProgressParams, IPartialItemsRequest<CodeLensContainer, CodeLens>
        {
            /// <summary>
            /// The document to request code lens for.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }
        }

        /// <summary>
        /// A code lens represents a command that should be shown along with
        /// source text, like the number of references, a way to run tests, etc.
        ///
        /// A code lens is _unresolved_ when no command is associated to it. For performance
        /// reasons the creation of a code lens and resolving should be done in two stages.
        /// </summary>
        [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
        [Parallel]
        [Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "CodeLensResolve"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient)),
            GenerateTypedData,
            GenerateContainer
        ]
        [RegistrationOptions(typeof(CodeLensRegistrationOptions)), Capability(typeof(CodeLensCapability))]
        public partial record CodeLens : IRequest<CodeLens>, ICanBeResolved, IDoesNotParticipateInRegistration
        {
            /// <summary>
            /// The range in which this code lens is valid. Should only span a single line.
            /// </summary>
            public Range Range { get; init; }

            /// <summary>
            /// The command this code lens represents.
            /// </summary>
            [Optional]
            public Command? Command { get; init; }

            /// <summary>
            /// A data entry field that is preserved on a code lens item between
            /// a code lens and a code lens resolve request.
            /// </summary>
            [Optional]
            public JToken? Data { get; init; }

            private string DebuggerDisplay => $"{Range}{( Command != null ? $" {Command}" : "" )}";

            /// <inheritdoc />
            public override string ToString() => DebuggerDisplay;
        }

        [GenerateRegistrationOptions(nameof(ServerCapabilities.CodeLensProvider))]
        [RegistrationOptionsConverter(typeof(CodeLensRegistrationOptionsConverter))]
        [RegistrationName(TextDocumentNames.CodeLens)]
        public partial class CodeLensRegistrationOptions : IWorkDoneProgressOptions, ITextDocumentRegistrationOptions
        {
            /// <summary>
            /// Code lens has a resolve provider as well.
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            private class CodeLensRegistrationOptionsConverter : RegistrationOptionsConverterBase<CodeLensRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public CodeLensRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.CodeLensProvider))
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(CodeLensRegistrationOptions source)
                {
                    return new() {
                        ResolveProvider = source.ResolveProvider || _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ICodeLensResolveHandler)),
                        WorkDoneProgress = source.WorkDoneProgress
                    };
                }
            }
        }
    }

    namespace Models.Proposals
    {
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(WorkspaceNames.CodeLensRefresh, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Proposals"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        [Capability(typeof(CodeLensWorkspaceClientCapabilities))]
        public partial record CodeLensRefreshParams : IRequest
        {
        }
    }

    namespace Client.Capabilities
    {
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.CodeLens))]
        public partial class CodeLensCapability : DynamicCapability, ConnectedCapability<ICodeLensHandler>
        {
        }

        /// <summary>
        /// Capabilities specific to the code lens requests scoped to the
        /// workspace.
        ///
        /// @since 3.16.0 - proposed state.
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.Workspace), nameof(WorkspaceClientCapabilities.CodeLens))]
        public class CodeLensWorkspaceClientCapabilities : ICapability
        {
            /// <summary>
            /// Whether the client implementation supports a refresh request send from the server
            /// to the client. This is useful if a server detects a change which requires a
            /// re-calculation of all code lenses.
            /// </summary>
            [Optional]
            public bool RefreshSupport { get; set; }
        }
    }

    namespace Document
    {
    }
}
