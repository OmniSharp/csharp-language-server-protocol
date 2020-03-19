using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class Serializer : JsonRpcSerializer, ISerializer
    {
        private static readonly CompletionItemKind[] DefaultCompletionItemKinds = Enum.GetValues(typeof(CompletionItemKind))
            .Cast<CompletionItemKind>()
            .Where(x => x < CompletionItemKind.Folder)
            .ToArray();

        private static readonly SymbolKind[] DefaultSymbolKinds = Enum.GetValues(typeof(SymbolKind))
            .Cast<SymbolKind>()
            .Where(x => x < SymbolKind.Key)
            .ToArray();

        public ClientVersion ClientVersion { get; private set; }

        public static Serializer Instance { get; } = new Serializer();
        public Serializer() : this(ClientVersion.Lsp3) { }
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
                DefaultSymbolKinds
            );
            return serializer;
        }

        protected override JsonSerializerSettings CreateSerializerSettings()
        {
            var settings = base.CreateSerializerSettings();
            settings.ContractResolver = new ContractResolver(
                DefaultCompletionItemKinds,
                DefaultSymbolKinds,
                DefaultSymbolKinds
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
            ReplaceConverter(converters, new SymbolInformationOrDocumentSymbolConverter());
            ReplaceConverter(converters, new LocationOrLocationLinkConverter());
            ReplaceConverter(converters, new WorkspaceEditDocumentChangeConverter());
            ReplaceConverter(converters, new ParameterInformationLabelConverter());
            ReplaceConverter(converters, new ValueTupleContractResolver<long, long>());
            base.AddOrReplaceConverters(converters);
        }

        public void SetClientCapabilities(ClientVersion clientVersion, ClientCapabilities clientCapabilities)
        {
            var completionItemKinds = DefaultCompletionItemKinds;
            var documentSymbolKinds = DefaultSymbolKinds;
            var workspaceSymbolKinds = DefaultSymbolKinds;

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
                var valueSet = symbol?.SymbolKind?.ValueSet;
                if (valueSet != null && valueSet.Any())
                {
                    documentSymbolKinds = valueSet.ToArray();
                }
            }

            if (clientCapabilities?.Workspace?.Symbol.IsSupported == true)
            {
                var symbol = clientCapabilities.Workspace.Symbol.Value;
                var valueSet = symbol?.SymbolKind?.ValueSet;
                if (valueSet != null && valueSet.Any())
                {
                    workspaceSymbolKinds = valueSet.ToArray();
                }
            }


            AddOrReplaceConverters(Settings.Converters);
            Settings.ContractResolver = new ContractResolver(
                completionItemKinds,
                documentSymbolKinds,
                workspaceSymbolKinds
            );

            AddOrReplaceConverters(JsonSerializer.Converters);
            JsonSerializer.ContractResolver = new ContractResolver(
                completionItemKinds,
                documentSymbolKinds,
                workspaceSymbolKinds
            );
        }
    }
}
