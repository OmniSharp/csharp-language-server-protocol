using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    public class Serializer : ISerializer
    {
        private readonly object _lock = new object();
        private long _id = 0;
        private static readonly CompletionItemKind[] DefaultCompletionItemKinds = Enum.GetValues(typeof(CompletionItemKind))
            .Cast<CompletionItemKind>()
            .Where(x => x < CompletionItemKind.Folder)
            .ToArray();

        private static readonly SymbolKind[] DefaultSymbolKinds = Enum.GetValues(typeof(SymbolKind))
            .Cast<SymbolKind>()
            .Where(x => x < SymbolKind.Key)
            .ToArray();

        public static Serializer Instance { get; } = new Serializer();
        public Serializer() : this(ClientVersion.Lsp3) { }
        public Serializer(ClientVersion clientVersion)
        {
            JsonSerializer = CreateSerializer(clientVersion);
            Settings = CreateSerializerSettings(clientVersion);
        }

        private static JsonSerializer CreateSerializer(ClientVersion version)
        {
            var serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new ContractResolver(
                DefaultCompletionItemKinds,
                DefaultSymbolKinds,
                DefaultSymbolKinds
            );
            AddOrReplaceConverters(serializer.Converters, version);

            return serializer;
        }

        private static JsonSerializerSettings CreateSerializerSettings(ClientVersion version)
        {
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();
            settings.ContractResolver = new ContractResolver(
                DefaultCompletionItemKinds,
                DefaultSymbolKinds,
                DefaultSymbolKinds
            );
            AddOrReplaceConverters(settings.Converters, version);

            return settings;
        }

        private static void AddOrReplaceConverters(ICollection<JsonConverter> converters, ClientVersion version)
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
            ReplaceConverter(converters, new MarkedStringsOrMarkupContentConverter());
            ReplaceConverter(converters, new CommandOrCodeActionConverter());
            ReplaceConverter(converters, new SymbolInformationOrDocumentSymbolConverter());
            ReplaceConverter(converters, new LocationOrLocationLinkConverter());
            ReplaceConverter(converters, new WorkspaceEditDocumentChangeConverter());
            ReplaceConverter(converters, new ParameterInformationLabelConverter());
            ReplaceConverter(converters, new ValueTupleContractResolver<long, long>());


            ReplaceConverter(converters, new ClientNotificationConverter());
            ReplaceConverter(converters, new ClientResponseConverter());
            ReplaceConverter(converters, new ClientRequestConverter());
            ReplaceConverter(converters, new RpcErrorConverter());
            ReplaceConverter(converters, new ErrorMessageConverter());
        }

        private static void ReplaceConverter<T>(ICollection<JsonConverter> converters, T item)
            where T : JsonConverter
        {
            var existingConverters = converters.OfType<T>().ToArray();
            if (existingConverters.Any())
            {
                foreach (var converter in existingConverters)
                    converters.Remove(converter);
            }
            converters.Add(item);
        }

        public JsonSerializer JsonSerializer { get; }

        public JsonSerializerSettings Settings { get; }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings);
        }

        public object DeserializeObject(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, Settings);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
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


            AddOrReplaceConverters(Settings.Converters, clientVersion);
            Settings.ContractResolver = new ContractResolver(
                completionItemKinds,
                documentSymbolKinds,
                workspaceSymbolKinds
            );

            AddOrReplaceConverters(JsonSerializer.Converters, clientVersion);
            JsonSerializer.ContractResolver = new ContractResolver(
                completionItemKinds,
                documentSymbolKinds,
                workspaceSymbolKinds
            );
        }
        public long GetNextId()
        {
            lock (_lock)
            {
                return _id++;
            }
        }
    }
}
