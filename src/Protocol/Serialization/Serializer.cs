using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
#pragma warning disable 618

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class LspSerializer : JsonRpcSerializer, ISerializer
    {
        private static readonly ImmutableArray<CompletionItemKind> DefaultCompletionItemKinds = Enum
                                                                                 .GetValues(typeof(CompletionItemKind))
                                                                                 .Cast<CompletionItemKind>()
                                                                                 .ToImmutableArray();

        private static readonly ImmutableArray<CompletionItemTag> DefaultCompletionItemTags = Enum
                                                                               .GetValues(typeof(CompletionItemTag))
                                                                               .Cast<CompletionItemTag>()
                                                                               .ToImmutableArray();

        private static readonly ImmutableArray<SymbolKind> DefaultSymbolKinds = Enum.GetValues(typeof(SymbolKind))
                                                                      .Cast<SymbolKind>()
                                                                      .ToImmutableArray();

        private static readonly ImmutableArray<SymbolTag> DefaultSymbolTags = Enum.GetValues(typeof(SymbolTag))
                                                                    .Cast<SymbolTag>()
                                                                    .ToImmutableArray();

        private static readonly ImmutableArray<DiagnosticTag> DefaultDiagnosticTags = Enum.GetValues(typeof(DiagnosticTag))
                                                                            .Cast<DiagnosticTag>()
                                                                            .ToImmutableArray();

        private static readonly ImmutableArray<CodeActionKind> DefaultCodeActionKinds = CodeActionKind.Defaults.ToImmutableArray();
        private static readonly ImmutableArray<SemanticTokenType> DefaultSemanticTokenType = SemanticTokenType.Defaults.ToImmutableArray();
        private static readonly ImmutableArray<SemanticTokenModifier> DefaultSemanticTokenModifiers = SemanticTokenModifier.Defaults.ToImmutableArray();


        private ImmutableArray<CompletionItemKind> _completionItemKinds = DefaultCompletionItemKinds;
        private ImmutableArray<CompletionItemTag> _completionItemTags = DefaultCompletionItemTags;
        private ImmutableArray<SymbolKind> _documentSymbolKinds = DefaultSymbolKinds;
        private ImmutableArray<SymbolTag> _documentSymbolTags = DefaultSymbolTags;
        private ImmutableArray<SymbolKind> _workspaceSymbolKinds = DefaultSymbolKinds;
        private ImmutableArray<SymbolTag> _workspaceSymbolTags = DefaultSymbolTags;
        private ImmutableArray<DiagnosticTag> _diagnosticTags = DefaultDiagnosticTags;
        private ImmutableArray<CodeActionKind> _codeActionKinds = DefaultCodeActionKinds;
        private ImmutableArray<SemanticTokenType> _semanticTokenTypes = DefaultSemanticTokenType;
        private ImmutableArray<SemanticTokenModifier> _semanticTokenModifier = DefaultSemanticTokenModifiers;

        // TODO: Add semantic tokens?

        public ClientVersion ClientVersion { get; }

        public static LspSerializer Instance { get; } = new LspSerializer();

        public LspSerializer() : this(ClientVersion.Lsp3)
        {
        }

        public LspSerializer(ClientVersion clientVersion) => ClientVersion = clientVersion;


        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.ContractResolver = new LspContractResolver(
                _completionItemKinds,
                _completionItemTags,
                _documentSymbolKinds,
                _workspaceSymbolKinds,
                _documentSymbolTags,
                _workspaceSymbolTags,
                _diagnosticTags,
                _codeActionKinds,
                _semanticTokenTypes,
                _semanticTokenModifier
            );
            return serializer;
        }

        protected override JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = base.CreateSerializerSettings();
            settings.ContractResolver = new LspContractResolver(
                _completionItemKinds,
                _completionItemTags,
                _documentSymbolKinds,
                _workspaceSymbolKinds,
                _documentSymbolTags,
                _workspaceSymbolTags,
                _diagnosticTags,
                _codeActionKinds,
                _semanticTokenTypes,
                _semanticTokenModifier
            );
            return settings;
        }

        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new SupportsConverter());
            ReplaceConverter(converters, new CompletionListConverter());
            ReplaceConverter(converters, new DiagnosticCodeConverter());
            ReplaceConverter(converters, new NullableDiagnosticCodeConverter());
            ReplaceConverter(converters, new LocationOrLocationLinksConverter());
            ReplaceConverter(converters, new MarkedStringCollectionConverter());
            ReplaceConverter(converters, new MarkedStringConverter());
            ReplaceConverter(converters, new StringOrMarkupContentConverter());
            ReplaceConverter(converters, new TextDocumentSyncConverter());
            ReplaceConverter(converters, new BooleanNumberStringConverter());
            ReplaceConverter(converters, new BooleanStringConverter());
            ReplaceConverter(converters, new BooleanOrConverter());
            ReplaceConverter(converters, new ProgressTokenConverter());
            ReplaceConverter(converters, new MarkedStringsOrMarkupContentConverter());
            ReplaceConverter(converters, new CommandOrCodeActionConverter());
            ReplaceConverter(converters, new SemanticTokensFullOrDeltaConverter());
            ReplaceConverter(converters, new SemanticTokensFullOrDeltaPartialResultConverter());
            ReplaceConverter(converters, new SymbolInformationOrDocumentSymbolConverter());
            ReplaceConverter(converters, new LocationOrLocationLinkConverter());
            ReplaceConverter(converters, new WorkspaceEditDocumentChangeConverter());
            ReplaceConverter(converters, new ParameterInformationLabelConverter());
            ReplaceConverter(converters, new ValueTupleContractResolver<long, long>());
            ReplaceConverter(converters, new RangeOrPlaceholderRangeConverter());
            ReplaceConverter(converters, new EnumLikeStringConverter());
            ReplaceConverter(converters, new DocumentUriConverter());
            //            ReplaceConverter(converters, new AggregateConverter<CodeLensContainer>());
            //            ReplaceConverter(converters, new AggregateConverter<DocumentLinkContainer>());
            //            ReplaceConverter(converters, new AggregateConverter<LocationContainer>());
            //            ReplaceConverter(converters, new AggregateConverter<LocationOrLocationLinks>());
            //            ReplaceConverter(converters, new AggregateConverter<CommandOrCodeActionContainer>());
            ReplaceConverter(converters, new AggregateCompletionListConverter());
            base.AddOrReplaceConverters(converters);
        }

        public LspSerializer WithCompletionItemKinds(IEnumerable<CompletionItemKind> completionItemKinds)
        {
            _completionItemKinds = completionItemKinds.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithCompletionItemTags(IEnumerable<CompletionItemTag> completionItemTags)
        {
            _completionItemTags = completionItemTags.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithDocumentSymbolKinds(IEnumerable<SymbolKind> documentSymbolKinds)
        {
            _documentSymbolKinds = documentSymbolKinds.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithDocumentSymbolTags(IEnumerable<SymbolTag> documentSymbolTags)
        {
            _documentSymbolTags = documentSymbolTags.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithWorkspaceSymbolKinds(IEnumerable<SymbolKind> workspaceSymbolKinds)
        {
            _workspaceSymbolKinds = workspaceSymbolKinds.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithWorkspaceSymbolTags(IEnumerable<SymbolTag> workspaceSymbolTags)
        {
            _workspaceSymbolTags = workspaceSymbolTags.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithDiagnosticTags(IEnumerable<DiagnosticTag> diagnosticTags)
        {
            _diagnosticTags = diagnosticTags.ToImmutableArray();
            return Reset();
        }

        public LspSerializer WithCodeActionKinds(IEnumerable<CodeActionKind> codeActionKinds)
        {
            _codeActionKinds = codeActionKinds.ToImmutableArray();
            return Reset();
        }

        public LspSerializer SetServerCapabilities(ServerCapabilities? serverCapabilities)
        {
            if (serverCapabilities?.CodeActionProvider?.IsValue == true)
            {
                var codeActions = serverCapabilities.CodeActionProvider.Value;
                var kindValueSet = codeActions?.CodeActionKinds;
                if (kindValueSet is not null)
                {
                    _codeActionKinds = kindValueSet.ToImmutableArray();
                }
            }

            return Reset();
        }

        public LspSerializer SetClientCapabilities(ClientCapabilities? clientCapabilities)
        {
            if (clientCapabilities?.TextDocument?.Completion.IsSupported == true)
            {
                var completion = clientCapabilities.TextDocument.Completion.Value;
                var valueSet = completion?.CompletionItemKind?.ValueSet;
                if (valueSet is not null)
                {
                    _completionItemKinds = valueSet.ToImmutableArray();
                }

                var tagSupportSet = completion?.CompletionItem?.TagSupport.Value?.ValueSet;
                if (tagSupportSet is not null)
                {
                    _completionItemTags = tagSupportSet.ToImmutableArray();
                }
            }

            if (clientCapabilities?.TextDocument?.DocumentSymbol.IsSupported == true)
            {
                var symbol = clientCapabilities.TextDocument.DocumentSymbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet is not null)
                {
                    _documentSymbolKinds = symbolKindSet.ToImmutableArray();
                }

                var valueSet = symbol?.TagSupport?.ValueSet;
                if (valueSet is not null)
                {
                    _documentSymbolTags = valueSet.ToImmutableArray();
                }
            }

            if (clientCapabilities?.Workspace?.Symbol.IsSupported == true)
            {
                var symbol = clientCapabilities.Workspace.Symbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet is not null)
                {
                    _workspaceSymbolKinds = symbolKindSet.ToImmutableArray();
                }

                var tagSupportSet = symbol?.TagSupport.Value?.ValueSet;
                if (tagSupportSet is not null)
                {
                    _workspaceSymbolTags = tagSupportSet.ToImmutableArray();
                }
            }

            if (clientCapabilities?.TextDocument?.PublishDiagnostics.IsSupported == true)
            {
                var publishDiagnostics = clientCapabilities.TextDocument?.PublishDiagnostics.Value;
                var tagValueSet = publishDiagnostics?.TagSupport.Value?.ValueSet;
                if (tagValueSet is not null)
                {
                    _diagnosticTags = tagValueSet.ToImmutableArray();
                }
            }

            if (clientCapabilities?.TextDocument?.CodeAction.IsSupported == true)
            {
                var codeActions = clientCapabilities.TextDocument?.CodeAction.Value;
                var kindValueSet = codeActions?.CodeActionLiteralSupport?.CodeActionKind.ValueSet;
                if (kindValueSet is not null)
                {
                    _codeActionKinds = kindValueSet.ToImmutableArray();
                }
            }

            return Reset();
        }

        private LspSerializer Reset()
        {
            AddOrReplaceConverters(Settings.Converters);
            Settings.ContractResolver = new LspContractResolver(
                _completionItemKinds,
                _completionItemTags,
                _documentSymbolKinds,
                _workspaceSymbolKinds,
                _documentSymbolTags,
                _workspaceSymbolTags,
                _diagnosticTags,
                _codeActionKinds,
                _semanticTokenTypes,
                _semanticTokenModifier
            );

            AddOrReplaceConverters(JsonSerializer.Converters);
            JsonSerializer.ContractResolver = new LspContractResolver(
                _completionItemKinds,
                _completionItemTags,
                _documentSymbolKinds,
                _workspaceSymbolKinds,
                _documentSymbolTags,
                _workspaceSymbolTags,
                _diagnosticTags,
                _codeActionKinds,
                _semanticTokenTypes,
                _semanticTokenModifier
            );
            return this;
        }
    }
}
