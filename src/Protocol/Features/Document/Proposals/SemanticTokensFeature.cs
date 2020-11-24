using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Proposals;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models.Proposals
    {
        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(TextDocumentNames.SemanticTokensFull, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals", Name = "SemanticTokensFull"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(SemanticTokensRegistrationOptions)), Capability(typeof(SemanticTokensCapability))]
        public partial class SemanticTokensParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                                    IPartialItemRequest<SemanticTokens?, SemanticTokensPartialResult>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(TextDocumentNames.SemanticTokensFullDelta, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(SemanticTokensRegistrationOptions)), Capability(typeof(SemanticTokensCapability))]
        public partial class SemanticTokensDeltaParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                                         IPartialItemRequest<SemanticTokensFullOrDelta?, SemanticTokensFullOrDeltaPartialResult>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The previous result id.
            /// </summary>
            public string PreviousResultId { get; set; } = null!;
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(TextDocumentNames.SemanticTokensRange, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(SemanticTokensRegistrationOptions)), Capability(typeof(SemanticTokensCapability))]
        public partial class SemanticTokensRangeParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                                         IPartialItemRequest<SemanticTokens?, SemanticTokensPartialResult>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; set; } = null!;

            /// <summary>
            /// The range the semantic tokens are requested for.
            /// </summary>
            public Range Range { get; set; } = null!;
        }

        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(WorkspaceNames.SemanticTokensRefresh, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Proposals"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        [Capability(typeof(SemanticTokensWorkspaceCapability))]
        public partial class SemanticTokensRefreshParams : IRequest
        {
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial class SemanticTokens : ISemanticTokenResult
        {
            public SemanticTokens()
            {
            }

            public SemanticTokens(SemanticTokensPartialResult partialResult)
            {
                Data = partialResult.Data;
            }

            /// <summary>
            /// An optional result id. If provided and clients support delta updating
            /// the client will include the result id in the next semantic token request.
            /// A server can then instead of computing all semantic tokens again simply
            /// send a delta.
            /// </summary>
            [Optional]
            public string? ResultId { get; set; }

            /// <summary>
            /// The actual tokens. For a detailed description about how the data is
            /// structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            public ImmutableArray<int> Data { get; set; }

            [return: NotNullIfNotNull("result")]
            public static SemanticTokens? From(SemanticTokensPartialResult? result) => result switch {
                not null => new SemanticTokens(result),
                _        => null
            };
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial class SemanticTokensPartialResult
        {
            /// <summary>
            /// The actual tokens. For a detailed description about how the data is
            /// structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
            /// </summary>
            public ImmutableArray<int> Data { get; set; }
        }

        [Obsolete(Constants.Proposal)]
        [RegistrationName(TextDocumentNames.SemanticTokensRegistration)]
        [GenerateRegistrationOptions(nameof(ServerCapabilities.SemanticTokensProvider))]
        [RegistrationOptionsConverter(typeof(SemanticTokensRegistrationOptionsConverter))]
        public partial class SemanticTokensRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions, IStaticRegistrationOptions
        {
            /// <summary>
            /// The legend used by the server
            /// </summary>
            public SemanticTokensLegend Legend { get; set; } = null!;

            /// <summary>
            /// Server supports providing semantic tokens for a specific range
            /// of a document.
            /// </summary>
            [Optional]
            public BooleanOr<SemanticTokensCapabilityRequestRange>? Range { get; set; }

            /// <summary>
            /// Server supports providing semantic tokens for a full document.
            /// </summary>
            [Optional]
            public BooleanOr<SemanticTokensCapabilityRequestFull>? Full { get; set; }

            class SemanticTokensRegistrationOptionsConverter : RegistrationOptionsConverterBase<SemanticTokensRegistrationOptions, StaticOptions>
            {
                private readonly IHandlersManager _handlersManager;

                public SemanticTokensRegistrationOptionsConverter(IHandlersManager handlersManager) : base(nameof(ServerCapabilities.SemanticTokensProvider))
                {
                    _handlersManager = handlersManager;
                }

                public override StaticOptions Convert(SemanticTokensRegistrationOptions source)
                {
                    var result = new StaticOptions {
                        WorkDoneProgress = source.WorkDoneProgress,
                        Legend = source.Legend,
                        Full = source.Full,
                        Range = source.Range
                    };
                    if (result.Full != null && result.Full?.Value.Delta != true)
                    {
                        var edits = _handlersManager.Descriptors.Any(z => z.HandlerType == typeof(ISemanticTokensDeltaHandler));
                        if (edits)
                        {
                            result.Full = new BooleanOr<SemanticTokensCapabilityRequestFull>(
                                new SemanticTokensCapabilityRequestFull {
                                    Delta = true
                                }
                            );
                        }
                    }

                    return result;
                }
            }
        }
    }

    namespace Client.Capabilities
    {
        /// <summary>
        /// Capabilities specific to the `textDocument/semanticTokens`
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.SemanticTokens))]
        public partial class SemanticTokensCapability : DynamicCapability, ConnectedCapability<ISemanticTokensFullHandler>,
                                                        ConnectedCapability<ISemanticTokensDeltaHandler>, ConnectedCapability<ISemanticTokensRangeHandler>,
                                                        ConnectedCapability<ISemanticTokensRefreshHandler>
        {
            /// <summary>
            /// Which requests the client supports and might send to the server.
            /// </summary>
            public SemanticTokensCapabilityRequests Requests { get; set; } = null!;

            /// <summary>
            /// The token types that the client supports.
            /// </summary>
            public Container<SemanticTokenType> TokenTypes { get; set; } = null!;

            /// <summary>
            /// The token modifiers that the client supports.
            /// </summary>
            public Container<SemanticTokenModifier> TokenModifiers { get; set; } = null!;

            /// <summary>
            /// The formats the clients supports.
            /// </summary>
            public Container<SemanticTokenFormat> Formats { get; set; } = null!;

            /// <summary>
            /// Whether the client supports tokens that can overlap each other.
            /// </summary>
            [Optional]
            public bool OverlappingTokenSupport { get; set; }

            /// <summary>
            /// Whether the client supports tokens that can span multiple lines.
            /// </summary>
            [Optional]
            public bool MultilineTokenSupport { get; set; }
        }

        public partial class SemanticTokensCapabilityRequests
        {
            /// <summary>
            /// The client will send the `textDocument/semanticTokens/range` request if
            /// the server provides a corresponding handler.
            /// </summary>
            [Optional]
            public Supports<SemanticTokensCapabilityRequestRange?> Range { get; set; }

            /// <summary>
            /// The client will send the `textDocument/semanticTokens/full` request if
            /// the server provides a corresponding handler.
            /// </summary>
            [Optional]
            public Supports<SemanticTokensCapabilityRequestFull?> Full { get; set; }
        }

        /// <summary>
        /// The client will send the `textDocument/semanticTokens/range` request if
        /// the server provides a corresponding handler.
        /// </summary>
        public partial class SemanticTokensCapabilityRequestRange
        {
        }

        /// <summary>
        /// The client will send the `textDocument/semanticTokens/full` request if
        /// the server provides a corresponding handler.
        /// </summary>
        public partial class SemanticTokensCapabilityRequestFull
        {
            /// <summary>
            /// The client will send the `textDocument/semanticTokens/full/delta` request if
            /// the server provides a corresponding handler.
            /// </summary>
            [Optional]
            public bool Delta { get; set; }
        }

        /// <summary>
        /// Capabilities specific to the semantic token requests scoped to the
        /// workspace.
        ///
        /// @since 3.16.0 - proposed state.
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(WorkspaceClientCapabilities.SemanticTokens))]
        public class SemanticTokensWorkspaceCapability : ICapability
        {
            /// <summary>
            /// Whether the client implementation supports a refresh request send from
            /// the server to the client. This is useful if a server detects a project
            /// wide configuration change which requires a re-calculation of all semantic
            /// tokens provided by the server issuing the request.
            /// </summary>
            [Optional]
            public bool RefreshSupport { get; set; }
        }
    }

    namespace Document.Proposals
    {
        [Obsolete(Constants.Proposal)]
        public abstract class SemanticTokensHandlerBase :
            AbstractHandlers.Base<SemanticTokensRegistrationOptions, SemanticTokensCapability>,
            ISemanticTokensFullHandler,
            ISemanticTokensDeltaHandler,
            ISemanticTokensRangeHandler
        {
            public virtual async Task<SemanticTokens?> Handle(SemanticTokensParams request, CancellationToken cancellationToken)
            {
                var document = await GetSemanticTokensDocument(request, cancellationToken).ConfigureAwait(false);
                var builder = document.Create();
                await Tokenize(builder, request, cancellationToken).ConfigureAwait(false);
                return builder.Commit().GetSemanticTokens();
            }

            public virtual async Task<SemanticTokensFullOrDelta?> Handle(SemanticTokensDeltaParams request, CancellationToken cancellationToken)
            {
                var document = await GetSemanticTokensDocument(request, cancellationToken).ConfigureAwait(false);
                var builder = document.Edit(request);
                await Tokenize(builder, request, cancellationToken).ConfigureAwait(false);
                return builder.Commit().GetSemanticTokensEdits();
            }

            public virtual async Task<SemanticTokens?> Handle(SemanticTokensRangeParams request, CancellationToken cancellationToken)
            {
                var document = await GetSemanticTokensDocument(request, cancellationToken).ConfigureAwait(false);
                var builder = document.Create();
                await Tokenize(builder, request, cancellationToken).ConfigureAwait(false);
                return builder.Commit().GetSemanticTokens(request.Range);
            }

            public virtual void SetCapability(SemanticTokensCapability capability) => Capability = capability;
            protected SemanticTokensCapability Capability { get; private set; } = null!;
            protected abstract Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken);
            protected abstract Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken);
        }

        [Obsolete(Constants.Proposal)]
        public static partial class SemanticTokensExtensions
        {
            private static SemanticTokensRegistrationOptions RegistrationOptionsFactory(SemanticTokensCapability capability)
            {
                var registrationOptions = new SemanticTokensRegistrationOptions {
                    Full = new SemanticTokensCapabilityRequestFull()
                };
                registrationOptions.Range ??= new SemanticTokensCapabilityRequestRange();
                if (registrationOptions.Full?.IsValue == true)
                {
                    registrationOptions.Full.Value.Delta = true;
                }

                // Ensure the legend is created properly.
                registrationOptions.Legend = new SemanticTokensLegend() {
                    TokenModifiers = SemanticTokenModifier.Defaults.Join(capability.TokenModifiers, z => z, z => z, (a, b) => a).ToArray(),
                    TokenTypes = SemanticTokenType.Defaults.Join(capability.TokenTypes, z => z, z => z, (a, b) => a).ToArray(),
                };

                return registrationOptions;
            }

            public static ILanguageServerRegistry OnSemanticTokens(
                this ILanguageServerRegistry registry,
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
                Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                Func<SemanticTokensCapability, SemanticTokensRegistrationOptions>? registrationOptionsFactory
            )
            {
                registrationOptionsFactory ??= RegistrationOptionsFactory;
                return registry.AddHandlers(
                    new DelegatingHandlerBase(
                        HandlerAdapter<SemanticTokensCapability, ITextDocumentIdentifierParams>.Adapt(tokenize),
                        HandlerAdapter<SemanticTokensCapability>.Adapt(getSemanticTokensDocument),
                        RegistrationAdapter<SemanticTokensCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnSemanticTokens(
                this ILanguageServerRegistry registry,
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, CancellationToken, Task> tokenize,
                Func<ITextDocumentIdentifierParams, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                Func<SemanticTokensCapability, SemanticTokensRegistrationOptions>? registrationOptionsFactory
            )
            {
                registrationOptionsFactory ??= RegistrationOptionsFactory;
                return registry.AddHandlers(
                    new DelegatingHandlerBase(
                        HandlerAdapter<SemanticTokensCapability, ITextDocumentIdentifierParams>.Adapt(tokenize),
                        HandlerAdapter<SemanticTokensCapability>.Adapt(getSemanticTokensDocument),
                        RegistrationAdapter<SemanticTokensCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            public static ILanguageServerRegistry OnSemanticTokens(
                this ILanguageServerRegistry registry,
                Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, Task> tokenize,
                Func<ITextDocumentIdentifierParams, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                Func<SemanticTokensCapability, SemanticTokensRegistrationOptions>? registrationOptionsFactory
            )
            {
                registrationOptionsFactory ??= RegistrationOptionsFactory;
                return registry.AddHandlers(
                    new DelegatingHandlerBase(
                        HandlerAdapter<SemanticTokensCapability, ITextDocumentIdentifierParams>.Adapt(tokenize),
                        HandlerAdapter<SemanticTokensCapability>.Adapt(getSemanticTokensDocument),
                        RegistrationAdapter<SemanticTokensCapability>.Adapt(registrationOptionsFactory)
                    )
                );
            }

            private class DelegatingHandlerBase : SemanticTokensHandlerBase
            {
                private readonly Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> _tokenize;
                private readonly Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> _getSemanticTokensDocument;
                private readonly Func<SemanticTokensCapability, SemanticTokensRegistrationOptions> _registrationOptionsFactory;

                public DelegatingHandlerBase(
                    Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
                    Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                    Func<SemanticTokensCapability, SemanticTokensRegistrationOptions> registrationOptionsFactory
                ) : base()
                {
                    _tokenize = tokenize;
                    _getSemanticTokensDocument = getSemanticTokensDocument;
                    _registrationOptionsFactory = registrationOptionsFactory;
                }

                protected override Task Tokenize(SemanticTokensBuilder builder, ITextDocumentIdentifierParams identifier, CancellationToken cancellationToken)
                    => _tokenize(builder, identifier, Capability, cancellationToken);

                protected override Task<SemanticTokensDocument> GetSemanticTokensDocument(ITextDocumentIdentifierParams @params, CancellationToken cancellationToken)
                    => _getSemanticTokensDocument(@params, Capability, cancellationToken);

                protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability) => _registrationOptionsFactory(capability);
            }

            public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens?> RequestSemanticTokens(
                this ITextDocumentLanguageClient mediator,
                SemanticTokensParams @params, CancellationToken cancellationToken = default
            ) =>
                mediator.ProgressManager.MonitorUntil(
                    @params, (partial, result) => new SemanticTokens {
                        Data = partial.Data,
                        ResultId = result?.ResultId
                    }, cancellationToken
                );

            public static IRequestProgressObservable<SemanticTokensFullOrDeltaPartialResult, SemanticTokensFullOrDelta?> RequestSemanticTokensDelta(
                this ITextDocumentLanguageClient mediator, SemanticTokensDeltaParams @params, CancellationToken cancellationToken = default
            ) =>
                mediator.ProgressManager.MonitorUntil(
                    @params, (partial, result) => {
                        if (partial.IsDelta)
                        {
                            return new SemanticTokensFullOrDelta(
                                new SemanticTokensDelta {
                                    Edits = partial.Delta!.Edits,
                                    ResultId = result?.Delta?.ResultId ?? result?.Full?.ResultId
                                }
                            );
                        }

                        if (partial.IsFull)
                        {
                            return new SemanticTokensFullOrDelta(
                                new SemanticTokens {
                                    Data = partial.Full!.Data,
                                    ResultId = result?.Full?.ResultId ?? result?.Delta?.ResultId
                                }
                            );
                        }

                        return new SemanticTokensFullOrDelta(new SemanticTokens());
                    }, cancellationToken
                );

            public static IRequestProgressObservable<SemanticTokensPartialResult, SemanticTokens?> RequestSemanticTokensRange(
                this ITextDocumentLanguageClient mediator,
                SemanticTokensRangeParams @params, CancellationToken cancellationToken = default
            ) =>
                mediator.ProgressManager.MonitorUntil(
                    @params, (partial, result) => new SemanticTokens {
                        Data = partial.Data,
                        ResultId = result?.ResultId
                    }, cancellationToken
                );
        }
    }
}
