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
            var defaultSerialzier = JsonSerializer.CreateDefault();
            defaultSerialzier.ContractResolver = new ContractResolver();

            defaultSerialzier.Converters.Add(new SupportsConverter());
            defaultSerialzier.Converters.Add(new CompletionListConverter());
            defaultSerialzier.Converters.Add(new DiagnosticCodeConverter());
            defaultSerialzier.Converters.Add(new LocationOrLocationsConverter());
            defaultSerialzier.Converters.Add(new MarkedStringCollectionConverter());
            defaultSerialzier.Converters.Add(new MarkedStringConverter());
            defaultSerialzier.Converters.Add(new StringOrMarkupContentConverter());
            defaultSerialzier.Converters.Add(new TextDocumentSyncConverter());
            defaultSerialzier.Converters.Add(new BooleanNumberStringConverter());

            return defaultSerialzier;
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

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttributes<OptionalAttribute>().Any())
            {
                property.NullValueHandling = NullValueHandling.Ignore;
                property.DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate;
            }

            return property;
        }
    }
}
