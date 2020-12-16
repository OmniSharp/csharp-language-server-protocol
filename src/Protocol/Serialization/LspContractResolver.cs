using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

#pragma warning disable 618

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization
{
    internal class LspContractResolver : DefaultContractResolver
    {
        private readonly ImmutableArray<CompletionItemKind> _completionItemKinds;
        private readonly ImmutableArray<CompletionItemTag> _completionItemTags;
        private readonly ImmutableArray<SymbolKind> _documentSymbolKinds;
        private readonly ImmutableArray<SymbolKind> _workspaceSymbolKinds;
        private readonly ImmutableArray<SymbolTag> _documentSymbolTags;
        private readonly ImmutableArray<SymbolTag> _workspaceSymbolTags;
        private readonly ImmutableArray<DiagnosticTag> _diagnosticTags;
        private readonly ImmutableArray<CodeActionKind> _codeActionKinds;
        private readonly ImmutableArray<SemanticTokenType> _semanticTokenType;
        private readonly ImmutableArray<SemanticTokenModifier> _semanticTokenModifier;

        public LspContractResolver(
            ImmutableArray<CompletionItemKind> completionItemKinds,
            ImmutableArray<CompletionItemTag> completionItemTags,
            ImmutableArray<SymbolKind> documentSymbolKinds,
            ImmutableArray<SymbolKind> workspaceSymbolKinds,
            ImmutableArray<SymbolTag> documentSymbolTags,
            ImmutableArray<SymbolTag> workspaceSymbolTags,
            ImmutableArray<DiagnosticTag> diagnosticTags,
            ImmutableArray<CodeActionKind> codeActionKinds,
            ImmutableArray<SemanticTokenType> semanticTokenType,
            ImmutableArray<SemanticTokenModifier> semanticTokenModifier
        )
        {
            _completionItemKinds = completionItemKinds;
            _completionItemTags = completionItemTags;
            _documentSymbolKinds = documentSymbolKinds;
            _workspaceSymbolKinds = workspaceSymbolKinds;
            _documentSymbolTags = documentSymbolTags;
            _workspaceSymbolTags = workspaceSymbolTags;
            _diagnosticTags = diagnosticTags;
            _codeActionKinds = codeActionKinds;
            _semanticTokenType = semanticTokenType;
            _semanticTokenModifier = semanticTokenModifier;
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
                                                    .GetProperty(nameof(Supports<object?>.IsSupported), BindingFlags.Public | BindingFlags.Instance);
                    property.NullValueHandling = NullValueHandling.Ignore;
                    property.GetIsSpecified = o => {
                        var propertyValue = property.ValueProvider.GetValue(o);
                        if (propertyValue == null) return false;
                        return isSupportedGetter?.GetValue(propertyValue) as bool? == true;
                    };
                }
            }

            return contract;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttributes<OptionalAttribute>(true).Any()
             || property.DeclaringType.Name.EndsWith("Capabilities")
            )
            {
                property.NullValueHandling = NullValueHandling.Ignore;
                property.DefaultValueHandling = DefaultValueHandling.Ignore;
            }

            if (typeof(ISupports).IsAssignableFrom(property.PropertyType))
            {
                property.ValueProvider = new SupportsValueProvider(property.ValueProvider);
            }

            if (property.DeclaringType == typeof(CompletionItem))
            {
                if (property.PropertyType == typeof(CompletionItemKind) && _completionItemKinds is { Length: > 0 })
                {
                    property.ValueProvider =
                        new RangeValueProvider<CompletionItemKind>(property.ValueProvider, _completionItemKinds);
                }

                if (property.PropertyType == typeof(Container<CompletionItemTag>) && _completionItemTags is { Length: > 0 })
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<CompletionItemTag>(property.ValueProvider, _completionItemTags);
                }
            }

            if (property.DeclaringType == typeof(DocumentSymbol))
            {
                if (property.PropertyType == typeof(SymbolKind) && _documentSymbolKinds is { Length: > 0 })
                {
                    property.ValueProvider =
                        new RangeValueProvider<SymbolKind>(property.ValueProvider, _documentSymbolKinds);
                }

                if (property.PropertyType == typeof(Container<SymbolTag>) && _documentSymbolTags is { Length: > 0 })
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<SymbolTag>(property.ValueProvider, _documentSymbolTags);
                }
            }

            if (property.DeclaringType == typeof(Diagnostic))
            {
                if (property.PropertyType == typeof(Container<DiagnosticTag>) && _diagnosticTags is { Length: > 0 })
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<DiagnosticTag>(property.ValueProvider, _diagnosticTags);
                }
            }

            if (property.DeclaringType == typeof(CodeAction))
            {
                if (property.PropertyType == typeof(CodeActionKind) && _codeActionKinds is { Length: > 0 })
                {
                    property.ValueProvider =
                        new RangeValueProvider<CodeActionKind>(property.ValueProvider, _codeActionKinds);
                }
            }

            if (property.DeclaringType == typeof(SymbolInformation))
            {
                if (property.PropertyType == typeof(SymbolKind) && _workspaceSymbolKinds is { Length: > 0 })
                {
                    property.ValueProvider =
                        new RangeValueProvider<SymbolKind>(property.ValueProvider, _workspaceSymbolKinds);
                }

                if (property.PropertyType == typeof(Container<SymbolTag>) && _workspaceSymbolTags is { Length: > 0 })
                {
                    property.ValueProvider =
                        new ArrayRangeValueProvider<SymbolTag>(property.ValueProvider, _workspaceSymbolTags);
                }
            }

            return property;
        }

        private class SupportsValueProvider : IValueProvider
        {
            private readonly IValueProvider _valueProvider;
            public SupportsValueProvider(IValueProvider valueProvider) => _valueProvider = valueProvider;

            public void SetValue(object target, object value) => _valueProvider.SetValue(target, value);

            public object? GetValue(object target) =>
                _valueProvider.GetValue(target) switch {
                    ISupports supports when supports.IsSupported => supports,
                    _                                            => null
                };
        }

        private class RangeValueProvider<T> : IValueProvider
            where T : struct
        {
            private readonly IValueProvider _valueProvider;
            private readonly ImmutableArray<T> _validValues;
            private readonly T _defaultValue;

            public RangeValueProvider(IValueProvider valueProvider, ImmutableArray<T> validValues)
            {
                _valueProvider = valueProvider;
                _validValues = validValues;
                _defaultValue = validValues[0];
            }

            public void SetValue(object target, object value) => _valueProvider.SetValue(target, value);

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

        private class ArrayRangeValueProvider<T> : IValueProvider
            where T : struct
        {
            private readonly IValueProvider _valueProvider;
            private readonly ImmutableArray<T> _validValues;

            public ArrayRangeValueProvider(IValueProvider valueProvider, ImmutableArray<T> validValues)
            {
                _valueProvider = valueProvider;
                _validValues = validValues;
            }

            public void SetValue(object target, object value) => _valueProvider.SetValue(target, value);

            public object? GetValue(object target)
            {
                var value = _valueProvider.GetValue(target);
                if (value is IEnumerable<T> values)
                {
                    return values.Join(_validValues, z => z, z => z, (a, _) => a).ToArray();
                }

                return null;
            }
        }
    }
}
