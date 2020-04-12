using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

#pragma warning disable 618

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    class ContractResolver : DefaultContractResolver
    {
        private readonly CompletionItemKind[] _completionItemKinds;
        private readonly SymbolKind[] _documentSymbolKinds;
        private readonly SymbolKind[] _workspaceSymbolKinds;
        private readonly SymbolTag[] _documentSymbolTags;
        private readonly SymbolTag[] _workspaceSymbolTags;

        public ContractResolver(
            CompletionItemKind[] completionItemKinds,
            SymbolKind[] documentSymbolKinds,
            SymbolKind[] workspaceSymbolKinds,
            SymbolTag[] documentSymbolTags,
            SymbolTag[] workspaceSymbolTags)
        {
            _completionItemKinds = completionItemKinds;
            _documentSymbolKinds = documentSymbolKinds;
            _workspaceSymbolKinds = workspaceSymbolKinds;
            _documentSymbolTags = documentSymbolTags;
            _workspaceSymbolTags = workspaceSymbolTags;
            NamingStrategy = new CamelCaseNamingStrategy(true, false, true);
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            if (objectType == typeof(WorkspaceClientCapabilities) ||
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
            if (member.GetCustomAttributes<OptionalAttribute>().Any()
                || property.DeclaringType.Name.EndsWith("Capabilities")
            )
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            if (property.DeclaringType == typeof(CompletionItem) && property.PropertyType == typeof(CompletionItemKind))
            {
                property.ValueProvider =
                    new RangeValueProvider<CompletionItemKind>(property.ValueProvider, _completionItemKinds);
            }

            if (property.DeclaringType == typeof(DocumentSymbol))
            {
                if (property.PropertyType == typeof(SymbolKind))
                {
                    property.ValueProvider =
                        new RangeValueProvider<SymbolKind>(property.ValueProvider, _documentSymbolKinds);
                }

                if (property.PropertyType == typeof(Container<SymbolTag>))
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<SymbolTag>(property.ValueProvider, _documentSymbolTags);
                }
            }

            if (property.DeclaringType == typeof(SymbolInformation))
            {
                if (property.PropertyType == typeof(SymbolKind))
                {
                    property.ValueProvider =
                        new RangeValueProvider<SymbolKind>(property.ValueProvider, _workspaceSymbolKinds);
                }

                if (property.PropertyType == typeof(Container<SymbolTag>))
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<SymbolTag>(property.ValueProvider, _workspaceSymbolTags);
                }
            }

            return property;
        }

        class RangeValueProvider<T> : IValueProvider
            where T : struct
        {
            private readonly IValueProvider _valueProvider;
            private readonly T[] _validValues;
            private readonly T _defaultValue;

            public RangeValueProvider(IValueProvider valueProvider, T[] validValues)
            {
                _valueProvider = valueProvider;
                _validValues = validValues;
                _defaultValue = validValues[0];
            }

            public void SetValue(object target, object value)
            {
                _valueProvider.SetValue(target, value);
            }

            public object GetValue(object target)
            {
                var value = _valueProvider.GetValue(target);
                if (value is T)
                {
                    return _validValues.Any(z => z.Equals(value)) ? value : _defaultValue;
                }

                return _defaultValue;
            }
        }

        class ArrayRangeValueProvider<T> : IValueProvider
            where T : struct
        {
            private readonly IValueProvider _valueProvider;
            private readonly T[] _validValues;
            private readonly T _defaultValue;

            public ArrayRangeValueProvider(IValueProvider valueProvider, T[] validValues)
            {
                _valueProvider = valueProvider;
                _validValues = validValues;
                _defaultValue = validValues[0];
            }

            public void SetValue(object target, object value)
            {
                _valueProvider.SetValue(target, value);
            }

            public object GetValue(object target)
            {
                var value = _valueProvider.GetValue(target);
                if (value is IEnumerable<T> values)
                {
                    return values.Join(_validValues, z => z, z => z, (a, b) => a).ToArray();
                }

                return null;
            }
        }
    }
}
