using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

#pragma warning disable 618

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    class ContractResolver : DefaultContractResolver
    {
        private readonly CompletionItemKind[] _completionItemKinds;
        private readonly CompletionItemTag[] _completionItemTags;
        private readonly SymbolKind[] _documentSymbolKinds;
        private readonly SymbolKind[] _workspaceSymbolKinds;
        private readonly SymbolTag[] _documentSymbolTags;
        private readonly SymbolTag[] _workspaceSymbolTags;
        private readonly DiagnosticTag[] _diagnosticTags;
        private readonly CodeActionKind[] _codeActionKinds;

        public ContractResolver(
            CompletionItemKind[] completionItemKinds,
            CompletionItemTag[] completionItemTags,
            SymbolKind[] documentSymbolKinds,
            SymbolKind[] workspaceSymbolKinds,
            SymbolTag[] documentSymbolTags,
            SymbolTag[] workspaceSymbolTags,
            DiagnosticTag[] diagnosticTags,
            CodeActionKind[] codeActionKinds)
        {
            _completionItemKinds = completionItemKinds;
            _completionItemTags = completionItemTags;
            _documentSymbolKinds = documentSymbolKinds;
            _workspaceSymbolKinds = workspaceSymbolKinds;
            _documentSymbolTags = documentSymbolTags;
            _workspaceSymbolTags = workspaceSymbolTags;
            _diagnosticTags = diagnosticTags;
            _codeActionKinds = codeActionKinds;
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

            if (typeof(ISupports).IsAssignableFrom(property.PropertyType))
            {
                property.ValueProvider = new SupportsValueProvider(property.ValueProvider);
            }

            if (property.DeclaringType == typeof(CompletionItem))
            {
                if (property.PropertyType == typeof(CompletionItemKind))
                {
                    property.ValueProvider =
                        new RangeValueProvider<CompletionItemKind>(property.ValueProvider, _completionItemKinds);
                }

                if (property.PropertyType == typeof(Container<CompletionItemTag>))
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<CompletionItemTag>(property.ValueProvider, _completionItemTags);
                }
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

            if (property.DeclaringType == typeof(Diagnostic))
            {
                if (property.PropertyType == typeof(Container<DiagnosticTag>))
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<DiagnosticTag>(property.ValueProvider, _diagnosticTags);
                }
            }

            if (property.DeclaringType == typeof(CodeAction))
            {
                if (property.PropertyType == typeof(CodeActionKind))
                {
                    property.ValueProvider =
                        new RangeValueProvider<CodeActionKind>(property.ValueProvider, _codeActionKinds);
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

        class SupportsValueProvider : IValueProvider
        {
            private readonly IValueProvider _valueProvider;
            public SupportsValueProvider(IValueProvider valueProvider)
            {
                _valueProvider = valueProvider;
            }
            public void SetValue(object target, object value)
            {
                _valueProvider.SetValue(target, value);
            }

            public object GetValue(object target)
            {
                return _valueProvider.GetValue(target) switch {
                    ISupports supports when supports.IsSupported => supports,
                    _ => null
                };
            }
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

            public ArrayRangeValueProvider(IValueProvider valueProvider, T[] validValues)
            {
                _valueProvider = valueProvider;
                _validValues = validValues;
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
