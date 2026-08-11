using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
#pragma warning disable 618

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class LspSerializer : SystemTextJsonSerializer, ISerializer
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

        public LspSerializer(ClientVersion clientVersion) : base(CreateBaseOptions())
        {
            ClientVersion = clientVersion;
            Reset();
        }

        private static JsonSerializerOptions CreateBaseOptions()
        {
            var resolver = new DefaultJsonTypeInfoResolver();
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                TypeInfoResolver = resolver,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            };
            return options;
        }

        protected virtual void AddOrReplaceConverters(IList<JsonConverter> converters)
        {
            ReplaceConverter(converters, new SupportsConverterFactory());
            ReplaceConverter(converters, new CompletionListConverter());
            ReplaceConverter(converters, new TypedCompletionListConverterFactory());
            ReplaceConverter(converters, new StringOrInlayHintLabelPartsConverter());
            ReplaceConverter(converters, new SystemTextJsonEnumLikeStringConverterFactory());
            ReplaceConverter(converters, new RangeOrEditRangeConverter());
            ReplaceConverter(converters, new StringOrStringValueConverter());
            ReplaceConverter(converters, new InlineCompletionListConverter());
            ReplaceConverter(converters, new InlineValueBaseConverter());
            ReplaceConverter(converters, new StringOrNotebookDocumentFilterConverter());
            ReplaceConverter(converters, new ContainerBaseConverterFactory());
            ReplaceConverter(converters, new StjDocumentUriConverter());
            ReplaceConverter(converters, new StjDiagnosticCodeConverter());
            ReplaceConverter(converters, new StjNullableDiagnosticCodeConverter());
            ReplaceConverter(converters, new StjLocationOrLocationLinksConverter());
            ReplaceConverter(converters, new StjLocationOrLocationLinkConverter());
            ReplaceConverter(converters, new StjMarkedStringConverter());
            ReplaceConverter(converters, new StjMarkedStringCollectionConverter());
            ReplaceConverter(converters, new StjStringOrMarkupContentConverter());
            ReplaceConverter(converters, new StjMarkedStringsOrMarkupContentConverter());
            ReplaceConverter(converters, new StjTextDocumentSyncConverter());
            ReplaceConverter(converters, new StjBooleanNumberStringConverter());
            ReplaceConverter(converters, new StjBooleanStringConverter());
            ReplaceConverter(converters, new StjBooleanOrConverterFactory());
            ReplaceConverter(converters, new StjProgressTokenConverter());
            ReplaceConverter(converters, new StjCommandOrCodeActionConverter());
            ReplaceConverter(converters, new StjSemanticTokensFullOrDeltaConverter());
            ReplaceConverter(converters, new StjSemanticTokensFullOrDeltaPartialResultConverter());
            ReplaceConverter(converters, new StjSymbolInformationOrDocumentSymbolConverter());
            ReplaceConverter(converters, new StjWorkspaceEditDocumentChangeConverter());
            ReplaceConverter(converters, new StjParameterInformationLabelConverter());
            ReplaceConverter(converters, new StjValueTupleLongLongConverter());
            ReplaceConverter(converters, new StjRangeOrPlaceholderRangeConverter());
            ReplaceConverter(converters, new StjChangeAnnotationIdentifierConverter());
            ReplaceConverter(converters, new StjAggregateCompletionListConverter());
            ReplaceConverter(converters, new StjLspAnyConverter());
            ReplaceConverter(converters, new StjObjectPrimitiveOrElementConverter());
            ReplaceConverter(converters, new StjOptionalBooleanConverter());
            ReplaceConverter(converters, new StjTextEditConverter());
            ReplaceConverter(converters, new StjAnnotatedTextEditConverter());
            ReplaceConverter(converters, new StjSnippetTextEditConverter());
            ReplaceConverter(converters, new StjTextEditOrInsertReplaceEditConverter());
            ReplaceConverter(converters, new StjDocumentDiagnosticReportConverterFactory());
            ReplaceConverter(converters, new StjRelatedDocumentDiagnosticReportConverterFactory());
            ReplaceConverter(converters, new StjWorkspaceDocumentDiagnosticReportConverterFactory());
            ReplaceConverter(converters, new StjRpcErrorConverterFactory());
            ReplaceConverter(converters, new StjWorkspaceFolderOrUriConverter());
            ReplaceConverter(converters, new StjGlobPatternConverter());
            ReplaceConverter(converters, new StjLocationOrFileLocationConverter());
            ReplaceConverter(converters, new EnumMemberStringEnumConverterFactory());
        }

        protected internal static void RemoveConverter<T>(IList<JsonConverter> converters)
        {
            for (var i = converters.Count - 1; i >= 0; i--)
            {
                if (converters[i] is T)
                {
                    converters.RemoveAt(i);
                }
            }
        }

        protected internal static void ReplaceConverter<T>(IList<JsonConverter> converters, T item) where T : JsonConverter
        {
            RemoveConverter<T>(converters);
            converters.Add(item);
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
            var options = CreateOptionsSnapshot(CreateBaseOptions());
            var resolver = new DefaultJsonTypeInfoResolver();
            resolver.Modifiers.Add(ConfigureTypeInfo);
            options.TypeInfoResolver = resolver;
            AddOrReplaceConverters(options.Converters);
            ReplaceOptions(options);
            return this;
        }

        private void ConfigureTypeInfo(JsonTypeInfo typeInfo)
        {
            if (typeof(ICapabilitiesBase).IsAssignableFrom(typeInfo.Type) || typeInfo.Type == typeof(MessageActionItem))
            {
                var extensionData = typeInfo.Properties.FirstOrDefault(z =>
                    ( z.AttributeProvider as MemberInfo )?.Name == nameof(ICapabilitiesBase.ExtensionData)
                );
                if (extensionData is not null)
                {
                    extensionData.IsExtensionData = true;
                }
            }

            foreach (var property in typeInfo.Properties)
            {
                var member = property.AttributeProvider as MemberInfo;
                var hasOptional = member?.GetCustomAttributes(typeof(OptionalAttribute), true).Any() == true;

                if (hasOptional || typeInfo.Type.Name.EndsWith("Capabilities", StringComparison.Ordinal))
                {
                    AppendShouldSerialize(property, (_, value) => !IsDefaultValue(value, property.PropertyType));
                }

                if (typeof(ISupports).IsAssignableFrom(property.PropertyType))
                {
                    AppendShouldSerialize(property, (_, value) => value is ISupports supports && supports.IsSupported);
                }

                ConfigureCapabilityPropertyFiltering(typeInfo.Type, property);
            }
        }

        private void ConfigureCapabilityPropertyFiltering(Type declaringType, JsonPropertyInfo property)
        {
            var originalGet = property.Get;
            if (originalGet is null) return;

            if (declaringType == typeof(CompletionItem) && property.PropertyType == typeof(CompletionItemKind) && _completionItemKinds.Length > 0)
            {
                property.Get = target => ClampEnumValue((CompletionItemKind) originalGet(target)!, _completionItemKinds);
            }

            if (declaringType == typeof(CompletionItem) && property.PropertyType == typeof(Container<CompletionItemTag>) && _completionItemTags.Length > 0)
            {
                property.Get = target => FilterContainer((IEnumerable<CompletionItemTag>?) originalGet(target), _completionItemTags);
            }

            if (declaringType == typeof(DocumentSymbol) && property.PropertyType == typeof(SymbolKind) && _documentSymbolKinds.Length > 0)
            {
                property.Get = target => ClampEnumValue((SymbolKind) originalGet(target)!, _documentSymbolKinds);
            }

            if (declaringType == typeof(DocumentSymbol) && property.PropertyType == typeof(Container<SymbolTag>) && _documentSymbolTags.Length > 0)
            {
                property.Get = target => FilterContainer((IEnumerable<SymbolTag>?) originalGet(target), _documentSymbolTags);
            }

            if (declaringType == typeof(Diagnostic) && property.PropertyType == typeof(Container<DiagnosticTag>) && _diagnosticTags.Length > 0)
            {
                property.Get = target => FilterContainer((IEnumerable<DiagnosticTag>?) originalGet(target), _diagnosticTags);
            }

            if (declaringType == typeof(CodeAction) && property.PropertyType == typeof(CodeActionKind) && _codeActionKinds.Length > 0)
            {
                property.Get = target => originalGet(target) is CodeActionKind value ? ClampEnumValue(value, _codeActionKinds) : null;
            }

            if (declaringType == typeof(SymbolInformation) && property.PropertyType == typeof(SymbolKind) && _workspaceSymbolKinds.Length > 0)
            {
                property.Get = target => ClampEnumValue((SymbolKind) originalGet(target)!, _workspaceSymbolKinds);
            }

            if (declaringType == typeof(SymbolInformation) && property.PropertyType == typeof(Container<SymbolTag>) && _workspaceSymbolTags.Length > 0)
            {
                property.Get = target => FilterContainer((IEnumerable<SymbolTag>?) originalGet(target), _workspaceSymbolTags);
            }
        }

        private static bool IsDefaultValue(object? value, Type type) =>
            value is null || type.IsValueType && value.Equals(Activator.CreateInstance(type));

        private static void AppendShouldSerialize(JsonPropertyInfo property, Func<object, object?, bool> shouldSerialize)
        {
            var existingShouldSerialize = property.ShouldSerialize;
            property.ShouldSerialize = existingShouldSerialize is null
                ? shouldSerialize
                : (target, value) => existingShouldSerialize(target, value) && shouldSerialize(target, value);
        }

        private static T ClampEnumValue<T>(T value, ImmutableArray<T> validValues) where T : struct
            => validValues.Contains(value) ? value : validValues[0];

        private static Container<T>? FilterContainer<T>(IEnumerable<T>? value, ImmutableArray<T> validValues) where T : struct
        {
            if (value is null) return null;
            return new Container<T>(value.Join(validValues, z => z, z => z, (left, _) => left));
        }
    }
}
