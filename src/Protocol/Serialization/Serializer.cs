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
    public static class Serializer
    {
        public static JsonSerializer CreateSerializer(ClientVersion version)
        {
            var serializer = JsonSerializer.CreateDefault();
            serializer.ContractResolver = new ContractResolver();

            serializer.Converters.Add(new SupportsConverter());
            serializer.Converters.Add(new CompletionListConverter());
            serializer.Converters.Add(new DiagnosticCodeConverter());
            serializer.Converters.Add(new LocationOrLocationsConverter());
            serializer.Converters.Add(new MarkedStringCollectionConverter());
            serializer.Converters.Add(new MarkedStringConverter());
            serializer.Converters.Add(new StringOrMarkupContentConverter());
            serializer.Converters.Add(new TextDocumentSyncConverter());
            serializer.Converters.Add(new BooleanNumberStringConverter());
            serializer.Converters.Add(new MarkedStringsOrMarkupContentConverter());

            return serializer;
        }

        public static JsonSerializerSettings CreateSerializerSettings(ClientVersion version)
        {
            var settings = JsonConvert.DefaultSettings != null ? JsonConvert.DefaultSettings() : new JsonSerializerSettings();

            settings.ContractResolver = new ContractResolver();

            settings.Converters.Add(new SupportsConverter());
            settings.Converters.Add(new CompletionListConverter());
            settings.Converters.Add(new DiagnosticCodeConverter());
            settings.Converters.Add(new LocationOrLocationsConverter());
            settings.Converters.Add(new MarkedStringCollectionConverter());
            settings.Converters.Add(new MarkedStringConverter());
            settings.Converters.Add(new StringOrMarkupContentConverter());
            settings.Converters.Add(new TextDocumentSyncConverter());
            settings.Converters.Add(new BooleanNumberStringConverter());
            settings.Converters.Add(new MarkedStringsOrMarkupContentConverter());

            return settings;
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
