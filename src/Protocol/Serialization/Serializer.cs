using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class Serializer : JsonRpcSerializer, ISerializer
    {
        private static readonly CompletionItemKind[] DefaultCompletionItemKinds = Enum
            .GetValues(typeof(CompletionItemKind))
            .Cast<CompletionItemKind>()
            .ToArray();

        private static readonly CompletionItemTag[] DefaultCompletionItemTags = Enum
            .GetValues(typeof(CompletionItemTag))
            .Cast<CompletionItemTag>()
            .ToArray();

        private static readonly SymbolKind[] DefaultSymbolKinds = Enum.GetValues(typeof(SymbolKind))
            .Cast<SymbolKind>()
            .ToArray();

        private static readonly SymbolTag[] DefaultSymbolTags = Enum.GetValues(typeof(SymbolTag))
            .Cast<SymbolTag>()
            .ToArray();

        private static readonly DiagnosticTag[] DefaultDiagnosticTags = Enum.GetValues(typeof(DiagnosticTag))
            .Cast<DiagnosticTag>()
            .ToArray();

        private static readonly CodeActionKind[] DefaultCodeActionKinds = typeof(CodeActionKind).GetFields(BindingFlags.Static | BindingFlags.Public)
            .Select(z => z.GetValue(null) )
            .Cast<CodeActionKind>()
            .ToArray();

        public ClientVersion ClientVersion { get; private set; }

        public static Serializer Instance { get; } = new Serializer();

        public Serializer() : this(ClientVersion.Lsp3)
        {
        }

        public Serializer(ClientVersion clientVersion)
        {
            ClientVersion = clientVersion;
        }


        protected override JsonSerializer CreateSerializer()
        {
            var serializer = base.CreateSerializer();
            serializer.ContractResolver = new ContractResolver(
                DefaultCompletionItemKinds,
                DefaultCompletionItemTags,
                DefaultSymbolKinds,
                DefaultSymbolKinds,
                DefaultSymbolTags,
                DefaultSymbolTags,
                DefaultDiagnosticTags,
                DefaultCodeActionKinds
            );
            return serializer;
        }

        protected override JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = base.CreateSerializerSettings();
            settings.ContractResolver = new ContractResolver(
                DefaultCompletionItemKinds,
                DefaultCompletionItemTags,
                DefaultSymbolKinds,
                DefaultSymbolKinds,
                DefaultSymbolTags,
                DefaultSymbolTags,
                DefaultDiagnosticTags,
                DefaultCodeActionKinds
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
            ReplaceConverter(converters, new SemanticTokensOrSemanticTokensEditsConverter());
            ReplaceConverter(converters, new SemanticTokensPartialResultOrSemanticTokensEditsPartialResultConverter());
            ReplaceConverter(converters, new SymbolInformationOrDocumentSymbolConverter());
            ReplaceConverter(converters, new LocationOrLocationLinkConverter());
            ReplaceConverter(converters, new WorkspaceEditDocumentChangeConverter());
            ReplaceConverter(converters, new ParameterInformationLabelConverter());
            ReplaceConverter(converters, new ValueTupleContractResolver<long, long>());
            ReplaceConverter(converters, new RangeOrPlaceholderRangeConverter());
            ReplaceConverter(converters, new EnumLikeStringConverter());
            ReplaceConverter(converters, new DocumentUriConverter());
            base.AddOrReplaceConverters(converters);
        }

        public void SetClientCapabilities(ClientVersion clientVersion, ClientCapabilities clientCapabilities)
        {
            var completionItemKinds = DefaultCompletionItemKinds;
            var completionItemTags = DefaultCompletionItemTags;
            var documentSymbolKinds = DefaultSymbolKinds;
            var documentSymbolTags = DefaultSymbolTags;
            var workspaceSymbolKinds = DefaultSymbolKinds;
            var workspaceSymbolTags = DefaultSymbolTags;
            var diagnosticTags = DefaultDiagnosticTags;
            var codeActionKinds = DefaultCodeActionKinds;

            if (clientCapabilities?.TextDocument?.Completion.IsSupported == true)
            {
                var completion = clientCapabilities.TextDocument.Completion.Value;
                var valueSet = completion?.CompletionItemKind?.ValueSet;
                if (valueSet != null)
                {
                    completionItemKinds = valueSet.ToArray();
                }

                var tagSupportSet = completion?.CompletionItem?.TagSupport.Value?.ValueSet;
                if (tagSupportSet != null)
                {
                    completionItemTags = tagSupportSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.DocumentSymbol.IsSupported == true)
            {
                var symbol = clientCapabilities.TextDocument.DocumentSymbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet != null)
                {
                    documentSymbolKinds = symbolKindSet.ToArray();
                }
                var valueSet = symbol?.TagSupport.Value?.ValueSet;
                if (valueSet != null)
                {
                    documentSymbolTags = valueSet.ToArray();
                }
            }

            if (clientCapabilities?.Workspace?.Symbol.IsSupported == true)
            {
                var symbol = clientCapabilities.Workspace.Symbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet != null)
                {
                    workspaceSymbolKinds = symbolKindSet.ToArray();
                }
                var tagSupportSet = symbol?.TagSupport.Value?.ValueSet;
                if (tagSupportSet != null)
                {
                    workspaceSymbolTags = tagSupportSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.PublishDiagnostics.IsSupported == true)
            {
                var publishDiagnostics = clientCapabilities?.TextDocument?.PublishDiagnostics.Value;
                var tagValueSet = publishDiagnostics.TagSupport.Value?.ValueSet;
                if (tagValueSet != null)
                {
                    diagnosticTags = tagValueSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.CodeAction.IsSupported == true)
            {
                var codeActions = clientCapabilities?.TextDocument?.CodeAction.Value;
                var kindValueSet = codeActions.CodeActionLiteralSupport?.CodeActionKind?.ValueSet;
                if (kindValueSet != null)
                {
                    codeActionKinds = kindValueSet.ToArray();
                }
            }


            AddOrReplaceConverters(Settings.Converters);
            Settings.ContractResolver = new ContractResolver(
                completionItemKinds,
                completionItemTags,
                documentSymbolKinds,
                workspaceSymbolKinds,
                documentSymbolTags,
                workspaceSymbolTags,
                diagnosticTags,
                codeActionKinds
            );

            AddOrReplaceConverters(JsonSerializer.Converters);
            JsonSerializer.ContractResolver = new ContractResolver(
                completionItemKinds,
                completionItemTags,
                documentSymbolKinds,
                workspaceSymbolKinds,
                documentSymbolTags,
                workspaceSymbolTags,
                diagnosticTags,
                codeActionKinds
            );
        }
    }
}
