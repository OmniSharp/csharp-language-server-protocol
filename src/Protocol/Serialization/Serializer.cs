using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class Serializer : JsonRpcSerializer, ISerializer
    {
        private static readonly CompletionItemKind[] DefaultCompletionItemKinds = Enum
            .GetValues(typeof(CompletionItemKind))
            .Cast<CompletionItemKind>()
            .ToArray();

        private static readonly SymbolKind[] DefaultSymbolKinds = Enum.GetValues(typeof(SymbolKind))
            .Cast<SymbolKind>()
            .ToArray();

        private static readonly SymbolTag[] DefaultSymbolTags = Enum.GetValues(typeof(SymbolTag))
            .Cast<SymbolTag>()
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
                DefaultSymbolKinds,
                DefaultSymbolKinds,
                DefaultSymbolTags,
                DefaultSymbolTags
            );
            return serializer;
        }

        protected override JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = base.CreateSerializerSettings();
            settings.ContractResolver = new ContractResolver(
                DefaultCompletionItemKinds,
                DefaultSymbolKinds,
                DefaultSymbolKinds,
                DefaultSymbolTags,
                DefaultSymbolTags
            );
            return settings;
        }

        protected override void AddOrReplaceConverters(ICollection<JsonConverter> converters)
        {
            ReplaceConverter(converters, new SupportsConverter());
            ReplaceConverter(converters, new CompletionListConverter());
            ReplaceConverter(converters, new DiagnosticCodeConverter());
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
            base.AddOrReplaceConverters(converters);
        }

        public void SetClientCapabilities(ClientVersion clientVersion, ClientCapabilities clientCapabilities)
        {
            var completionItemKinds = DefaultCompletionItemKinds;
            var documentSymbolKinds = DefaultSymbolKinds;
            var documentSymbolTags = DefaultSymbolTags;
            var workspaceSymbolKinds = DefaultSymbolKinds;
            var workspaceSymbolTags = DefaultSymbolTags;

            if (clientCapabilities?.TextDocument?.Completion.IsSupported == true)
            {
                var completion = clientCapabilities.TextDocument.Completion.Value;
                var valueSet = completion?.CompletionItemKind?.ValueSet;
                if (valueSet != null && valueSet.Any())
                {
                    completionItemKinds = valueSet.ToArray();
                }
            }

            if (clientCapabilities?.TextDocument?.DocumentSymbol.IsSupported == true)
            {
                var symbol = clientCapabilities.TextDocument.DocumentSymbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet != null && symbolKindSet.Any())
                {
                    documentSymbolKinds = symbolKindSet.ToArray();
                }
                var valueSet = symbol?.TagSupport?.ValueSet;
                if (valueSet != null && valueSet.Any())
                {
                    documentSymbolTags = valueSet.ToArray();
                }
            }

            if (clientCapabilities?.Workspace?.Symbol.IsSupported == true)
            {
                var symbol = clientCapabilities.Workspace.Symbol.Value;
                var symbolKindSet = symbol?.SymbolKind?.ValueSet;
                if (symbolKindSet != null && symbolKindSet.Any())
                {
                    workspaceSymbolKinds = symbolKindSet.ToArray();
                }
                var tagSupportSet = symbol?.TagSupport?.ValueSet;
                if (tagSupportSet != null && tagSupportSet.Any())
                {
                    workspaceSymbolTags = tagSupportSet.ToArray();
                }
            }


            AddOrReplaceConverters(Settings.Converters);
            Settings.ContractResolver = new ContractResolver(
                completionItemKinds,
                documentSymbolKinds,
                workspaceSymbolKinds,
                documentSymbolTags,
                workspaceSymbolTags
            );

            AddOrReplaceConverters(JsonSerializer.Converters);
            JsonSerializer.ContractResolver = new ContractResolver(
                completionItemKinds,
                documentSymbolKinds,
                workspaceSymbolKinds,
                documentSymbolTags,
                workspaceSymbolTags
            );
        }
    }
}
