using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
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
        public partial record SemanticTokensParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                                    IPartialItemRequest<SemanticTokens?, SemanticTokensPartialResult>
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }
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
        public partial record SemanticTokensDeltaParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                                         IPartialItemRequest<SemanticTokensFullOrDelta?, SemanticTokensFullOrDeltaPartialResult>, IDoesNotParticipateInRegistration
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The previous result id.
            /// </summary>
            public string PreviousResultId { get; init; }
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
        public partial record SemanticTokensRangeParams : IWorkDoneProgressParams, ITextDocumentIdentifierParams,
                                                         IPartialItemRequest<SemanticTokens?, SemanticTokensPartialResult>, IDoesNotParticipateInRegistration
        {
            /// <summary>
            /// The text document.
            /// </summary>
            public TextDocumentIdentifier TextDocument { get; init; }

            /// <summary>
            /// The range the semantic tokens are requested for.
            /// </summary>
            public Range Range { get; init; }
        }

        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(WorkspaceNames.SemanticTokensRefresh, Direction.ServerToClient)]
        [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Workspace.Proposals"), GenerateHandlerMethods,
         GenerateRequestMethods(typeof(IWorkspaceLanguageServer), typeof(ILanguageServer))]
        [Capability(typeof(SemanticTokensWorkspaceCapability))]
        public partial record SemanticTokensRefreshParams : IRequest
        {
        }

        [Obsolete(Constants.Proposal)]
        public interface ISemanticTokenResult
        {
            /// <summary>
            /// An optional result id. If provided and clients support delta updating
            /// the client will include the result id in the next semantic token request.
            /// A server can then instead of computing all semantic tokens again simply
            /// send a delta.
            /// </summary>
            [Optional]
            public string? ResultId { get; init; }
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial record SemanticTokens : ISemanticTokenResult
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
            public string? ResultId { get; init; }

            /// <summary>
            /// The actual tokens. For a detailed description about how the data is
            /// structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            public ImmutableArray<int> Data { get; init; }

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
        public partial record SemanticTokensPartialResult
        {
            /// <summary>
            /// The actual tokens. For a detailed description about how the data is
            /// structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
            /// </summary>
            public ImmutableArray<int> Data { get; init; }
        }


        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public record SemanticTokensDelta : ISemanticTokenResult
        {
            public SemanticTokensDelta()
            {
            }

            public SemanticTokensDelta(SemanticTokensDeltaPartialResult partialResult)
            {
                Edits = partialResult.Edits;
            }

            /// <summary>
            /// An optional result id. If provided and clients support delta updating
            /// the client will include the result id in the next semantic token request.
            /// A server can then instead of computing all semantic tokens again simply
            /// send a delta.
            /// </summary>
            [Optional]
            public string? ResultId { get; init; }

            /// <summary>
            /// For a detailed description how these edits are structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L131
            /// </summary>
            public Container<SemanticTokensEdit> Edits { get; init; }
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public record SemanticTokensDeltaPartialResult
        {
            /// <summary>
            /// The actual tokens. For a detailed description about how the data is
            /// structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
            /// </summary>
            public Container<SemanticTokensEdit> Edits { get; init; }
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public record SemanticTokensEdit
        {
            /// <summary>
            /// The start index of the edit
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            public int Start { get; init; }

            /// <summary>
            /// The number of items to delete
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            public int DeleteCount { get; init; }

            /// <summary>
            /// The actual tokens. For a detailed description about how the data is
            /// structured pls see
            /// https://github.com/microsoft/vscode-extension-samples/blob/5ae1f7787122812dcc84e37427ca90af5ee09f14/semantic-tokens-sample/vscode.proposed.d.ts#L71
            /// </summary>
            /// <remarks>
            /// <see cref="uint"/> in the LSP spec
            /// </remarks>
            [Optional]
            public ImmutableArray<int>? Data { get; init; } = ImmutableArray<int>.Empty;
        }

        [Obsolete(Constants.Proposal)]
        [JsonConverter(typeof(SemanticTokensFullOrDeltaConverter))]
        public record SemanticTokensFullOrDelta
        {
            public SemanticTokensFullOrDelta(SemanticTokensDelta delta)
            {
                Delta = delta;
                Full = null;
            }

            public SemanticTokensFullOrDelta(SemanticTokens full)
            {
                Delta = null;
                Full = full;
            }

            public SemanticTokensFullOrDelta(SemanticTokensFullOrDeltaPartialResult partialResult)
            {
                Full = null;
                Delta = null;

                if (partialResult.IsDelta)
                {
                    Delta = new SemanticTokensDelta(partialResult.Delta!) {
                        Edits = partialResult.Delta!.Edits
                    };
                }

                if (partialResult.IsFull)
                {
                    Full = new SemanticTokens(partialResult.Full!);
                }
            }

            public bool IsFull => Full != null;
            public SemanticTokens? Full { get; init; }

            public bool IsDelta => Delta != null;
            public SemanticTokensDelta? Delta { get;init; }

            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokensDelta")]
            public static SemanticTokensFullOrDelta? From(SemanticTokensDelta? semanticTokensDelta) => semanticTokensDelta switch {
                not null => new(semanticTokensDelta),
                _        => null
            };

            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokensDelta")]
            public static implicit operator SemanticTokensFullOrDelta?(SemanticTokensDelta? semanticTokensDelta) => semanticTokensDelta switch {
                not null => new(semanticTokensDelta),
                _        => null
            };

            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
            public static SemanticTokensFullOrDelta? From(SemanticTokens? semanticTokens) => semanticTokens switch {
                not null => new(semanticTokens),
                _        => null
            };

            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
            public static implicit operator SemanticTokensFullOrDelta?(SemanticTokens? semanticTokens) => semanticTokens switch {
                not null => new(semanticTokens),
                _        => null
            };

            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
            public static SemanticTokensFullOrDelta? From(SemanticTokensFullOrDeltaPartialResult? semanticTokens) => semanticTokens switch {
                not null => new(semanticTokens),
                _        => null
            };

            [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("semanticTokens")]
            public static implicit operator SemanticTokensFullOrDelta?(SemanticTokensFullOrDeltaPartialResult? semanticTokens) =>
                semanticTokens switch {
                    not null => new(semanticTokens),
                    _        => null
                };
        }

        [Obsolete(Constants.Proposal)]
        [JsonConverter(typeof(SemanticTokensFullOrDeltaPartialResultConverter))]
        public record SemanticTokensFullOrDeltaPartialResult
        {
            public SemanticTokensFullOrDeltaPartialResult(
                SemanticTokensPartialResult full
            )
            {
                Full = full;
                Delta = null;
            }

            public SemanticTokensFullOrDeltaPartialResult(
                SemanticTokensDeltaPartialResult delta
            )
            {
                Full = null;
                Delta = delta;
            }

            public bool IsDelta => Delta != null;
            public SemanticTokensDeltaPartialResult? Delta { get; }

            public bool IsFull => Full != null;
            public SemanticTokensPartialResult? Full { get; }

            public static implicit operator SemanticTokensFullOrDeltaPartialResult(SemanticTokensPartialResult semanticTokensPartialResult) =>
                new SemanticTokensFullOrDeltaPartialResult(semanticTokensPartialResult);

            public static implicit operator SemanticTokensFullOrDeltaPartialResult(SemanticTokensDeltaPartialResult semanticTokensDeltaPartialResult) =>
                new SemanticTokensFullOrDeltaPartialResult(semanticTokensDeltaPartialResult);

            public static implicit operator SemanticTokensFullOrDelta(SemanticTokensFullOrDeltaPartialResult semanticTokensDeltaPartialResult) =>
                new SemanticTokensFullOrDelta(semanticTokensDeltaPartialResult);
        }

        /// <summary>
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public record SemanticTokensLegend
        {
            private ImmutableDictionary<SemanticTokenModifier, int>? _tokenModifiersData;
            private ImmutableDictionary<SemanticTokenType, int>? _tokenTypesData;

            /// <summary>
            /// The token types a server uses.
            /// </summary>
            public Container<SemanticTokenType> TokenTypes { get; init; } = new Container<SemanticTokenType>(SemanticTokenType.Defaults);

            /// <summary>
            /// The token modifiers a server uses.
            /// </summary>
            public Container<SemanticTokenModifier> TokenModifiers { get; init; } = new Container<SemanticTokenModifier>(SemanticTokenModifier.Defaults);

            public int GetTokenTypeIdentity(string tokenType)
            {
                EnsureTokenTypes();
                if (string.IsNullOrWhiteSpace(tokenType)) return 0;
                return _tokenTypesData != null && _tokenTypesData.TryGetValue(tokenType, out var tokenTypeNumber) ? tokenTypeNumber : 0;
            }

            public int GetTokenTypeIdentity(SemanticTokenType? tokenType)
            {
                EnsureTokenTypes();
                if (!tokenType.HasValue) return 0;
                if (string.IsNullOrWhiteSpace(tokenType.Value)) return 0;
                return _tokenTypesData != null && _tokenTypesData.TryGetValue(tokenType.Value, out var tokenTypeNumber) ? tokenTypeNumber : 0;
            }

            public int GetTokenModifiersIdentity(params string[]? tokenModifiers)
            {
                EnsureTokenModifiers();
                if (tokenModifiers == null) return 0;
                return tokenModifiers
                      .Where(z => !string.IsNullOrWhiteSpace(z))
                      .Aggregate(
                           0,
                           (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                               ? acc + tokenModifer
                               : acc
                       );
            }

            public int GetTokenModifiersIdentity(IEnumerable<string>? tokenModifiers)
            {
                EnsureTokenModifiers();
                if (tokenModifiers == null) return 0;
                return tokenModifiers
                      .Where(z => !string.IsNullOrWhiteSpace(z))
                      .Aggregate(
                           0,
                           (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                               ? acc + tokenModifer
                               : acc
                       );
            }

            public int GetTokenModifiersIdentity(params SemanticTokenModifier[]? tokenModifiers)
            {
                EnsureTokenModifiers();
                if (tokenModifiers == null) return 0;
                return tokenModifiers
                      .Where(z => !string.IsNullOrWhiteSpace(z))
                      .Aggregate(
                           0,
                           (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                               ? acc + tokenModifer
                               : acc
                       );
            }

            public int GetTokenModifiersIdentity(IEnumerable<SemanticTokenModifier>? tokenModifiers)
            {
                EnsureTokenModifiers();
                if (tokenModifiers == null) return 0;
                return tokenModifiers
                      .Where(z => !string.IsNullOrWhiteSpace(z))
                      .Aggregate(
                           0,
                           (acc, value) => _tokenModifiersData != null && _tokenModifiersData.TryGetValue(value, out var tokenModifer)
                               ? acc + tokenModifer
                               : acc
                       );
            }

            private void EnsureTokenTypes() =>
                _tokenTypesData ??= TokenTypes
                                   .Select(
                                        (value, index) => (
                                            value: new SemanticTokenType(value),
                                            index
                                        )
                                    )
                                   .Where(z => !string.IsNullOrWhiteSpace(z.value))
                                   .ToImmutableDictionary(z => z.value, z => z.index);

            private void EnsureTokenModifiers() =>
                _tokenModifiersData ??= TokenModifiers
                                       .Select(
                                            (value, index) => (
                                                value: new SemanticTokenModifier(value),
                                                index
                                            )
                                        )
                                       .Where(z => !string.IsNullOrWhiteSpace(z.value))
                                       .ToImmutableDictionary(z => z.value, z => Convert.ToInt32(Math.Pow(2, z.index)));
        }

        /// <summary>
        /// The protocol defines an additional token format capability to allow future extensions of the format.
        /// The only format that is currently specified is `relative` expressing that the tokens are described using relative positions.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [StringEnum]
        public readonly partial struct SemanticTokenFormat
        {
            public static SemanticTokenFormat Relative { get; } = new SemanticTokenFormat("relative");
        }

        /// <summary>
        /// A set of predefined token modifiers. This set is not fixed
        /// an clients can specify additional token types via the
        /// corresponding client capabilities.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [StringEnum]
        public readonly partial struct SemanticTokenModifier
        {
            public static SemanticTokenModifier Documentation { get; } = new SemanticTokenModifier("documentation");
            public static SemanticTokenModifier Declaration { get; } = new SemanticTokenModifier("declaration");
            public static SemanticTokenModifier Definition { get; } = new SemanticTokenModifier("definition");
            public static SemanticTokenModifier Static { get; } = new SemanticTokenModifier("static");
            public static SemanticTokenModifier Async { get; } = new SemanticTokenModifier("async");
            public static SemanticTokenModifier Abstract { get; } = new SemanticTokenModifier("abstract");
            public static SemanticTokenModifier Deprecated { get; } = new SemanticTokenModifier("deprecated");
            public static SemanticTokenModifier Readonly { get; } = new SemanticTokenModifier("readonly");
            public static SemanticTokenModifier Modification { get; } = new SemanticTokenModifier("modification");
            public static SemanticTokenModifier DefaultLibrary { get; } = new SemanticTokenModifier("defaultLibrary");
        }


        /// <summary>
        /// A set of predefined token types. This set is not fixed
        /// an clients can specify additional token types via the
        /// corresponding client capabilities.
        ///
        /// @since 3.16.0
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [StringEnum]
        public readonly partial struct SemanticTokenType
        {
            public static SemanticTokenType Comment { get; } = new SemanticTokenType("comment");
            public static SemanticTokenType Keyword { get; } = new SemanticTokenType("keyword");
            public static SemanticTokenType String { get; } = new SemanticTokenType("string");
            public static SemanticTokenType Number { get; } = new SemanticTokenType("number");
            public static SemanticTokenType Regexp { get; } = new SemanticTokenType("regexp");
            public static SemanticTokenType Operator { get; } = new SemanticTokenType("operator");
            public static SemanticTokenType Namespace { get; } = new SemanticTokenType("namespace");
            public static SemanticTokenType Type { get; } = new SemanticTokenType("type");
            public static SemanticTokenType Struct { get; } = new SemanticTokenType("struct");
            public static SemanticTokenType Class { get; } = new SemanticTokenType("class");
            public static SemanticTokenType Interface { get; } = new SemanticTokenType("interface");
            public static SemanticTokenType Enum { get; } = new SemanticTokenType("enum");
            public static SemanticTokenType TypeParameter { get; } = new SemanticTokenType("typeParameter");
            public static SemanticTokenType Function { get; } = new SemanticTokenType("function");
            public static SemanticTokenType Method { get; } = new SemanticTokenType("method");
            public static SemanticTokenType Property { get; } = new SemanticTokenType("property");
            public static SemanticTokenType Macro { get; } = new SemanticTokenType("macro");
            public static SemanticTokenType Variable { get; } = new SemanticTokenType("variable");
            public static SemanticTokenType Parameter { get; } = new SemanticTokenType("parameter");
            public static SemanticTokenType Label { get; } = new SemanticTokenType("label");
            public static SemanticTokenType Modifier { get; } = new SemanticTokenType("modifier");
            public static SemanticTokenType Event { get; } = new SemanticTokenType("event");
            public static SemanticTokenType EnumMember { get; } = new SemanticTokenType("enumMember");
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
            private static SemanticTokensRegistrationOptions RegistrationOptionsFactory(SemanticTokensCapability capability, ClientCapabilities clientCapabilities)
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
                RegistrationOptionsDelegate<SemanticTokensRegistrationOptions, SemanticTokensCapability>? registrationOptionsFactory
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
                RegistrationOptionsDelegate<SemanticTokensRegistrationOptions, SemanticTokensCapability>? registrationOptionsFactory
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
                RegistrationOptionsDelegate<SemanticTokensRegistrationOptions, SemanticTokensCapability>? registrationOptionsFactory
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
                private readonly RegistrationOptionsDelegate<SemanticTokensRegistrationOptions, SemanticTokensCapability> _registrationOptionsFactory;

                public DelegatingHandlerBase(
                    Func<SemanticTokensBuilder, ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task> tokenize,
                    Func<ITextDocumentIdentifierParams, SemanticTokensCapability, CancellationToken, Task<SemanticTokensDocument>> getSemanticTokensDocument,
                    RegistrationOptionsDelegate<SemanticTokensRegistrationOptions, SemanticTokensCapability> registrationOptionsFactory
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

                protected internal override SemanticTokensRegistrationOptions CreateRegistrationOptions(SemanticTokensCapability capability, ClientCapabilities clientCapabilities) =>
                    _registrationOptionsFactory(capability, clientCapabilities);
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
