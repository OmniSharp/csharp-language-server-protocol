using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Converters;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public class Serializer : ISerializer
    {
        public Serializer() : this(ClientVersion.Lsp3) { }
        public Serializer(ClientVersion clientVersion)
        {
            JsonSerializer = CreateSerializer(clientVersion);
            Settings = CreateSerializerSettings(clientVersion);
        }

        private static JsonSerializer CreateSerializer(ClientVersion version)
        {
            var serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new ContractResolver();
            AddOrReplaceConverters(serializer.Converters, version);

            return serializer;
        }

        private static JsonSerializerSettings CreateSerializerSettings(ClientVersion version)
        {
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();
            settings.ContractResolver = new ContractResolver();
            AddOrReplaceConverters(settings.Converters, version);

            return settings;
        }

        private static void AddOrReplaceConverters(ICollection<JsonConverter> converters, ClientVersion version)
        {
            ReplaceConverter(converters, new SupportsConverter());
            ReplaceConverter(converters, new CompletionListConverter());
            ReplaceConverter(converters, new DiagnosticCodeConverter());
            ReplaceConverter(converters, new LocationOrLocationsConverter());
            ReplaceConverter(converters, new MarkedStringCollectionConverter());
            ReplaceConverter(converters, new MarkedStringConverter());
            ReplaceConverter(converters, new StringOrMarkupContentConverter());
            ReplaceConverter(converters, new TextDocumentSyncConverter());
            ReplaceConverter(converters, new BooleanNumberStringConverter());
            ReplaceConverter(converters, new MarkedStringsOrMarkupContentConverter());
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

        public object DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json, Settings);
        }

        public T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, Settings);
        }

        public Serializer SetClientVersion(ClientVersion clientVersion)
        {
            AddOrReplaceConverters(Settings.Converters, clientVersion);
            AddOrReplaceConverters(JsonSerializer.Converters, clientVersion);
            return this;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    class OptionalAttribute : Attribute { }

    class ContractResolver : DefaultContractResolver
    {
        public ContractResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy(true, false, true);
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            if (objectType == typeof(WorkspaceClientCapabilites) ||
                objectType == typeof(TextDocumentClientCapabilities))
            {
                foreach (var property in contract.Properties)
                {
                    var isSupportedGetter = property.PropertyType.GetTypeInfo()
                        .GetProperty(nameof(Supports<object>.IsSupported), BindingFlags.Public | BindingFlags.Instance);
                    property.NullValueHandling = NullValueHandling.Ignore;
                    property.GetIsSpecified = o => {
                        var propertyValue = property.ValueProvider.GetValue(o);
                        if (propertyValue == null) return false;
                        return isSupportedGetter.GetValue(propertyValue) as bool? == true;
                    };
                }
            }
            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            //if (property.DeclaringType.Name.EndsWith("Capability")) return property;
            // if (property.PropertyType.GetTypeInfo().IsGenericType)
            if (
               member.GetCustomAttributes<OptionalAttribute>().Any()
            || property.DeclaringType.Name.EndsWith("Capabilities")
            )
            {
                property.NullValueHandling = NullValueHandling.Ignore;
                // property.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            }

            return property;
        }
    }
}
