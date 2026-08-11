using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    internal sealed class SupportsConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Supports<>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var target = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter) Activator.CreateInstance(typeof(SupportsConverter<>).MakeGenericType(target))!;
        }

        private sealed class SupportsConverter<T> : JsonConverter<Supports<T>>
        {
            public override Supports<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                {
                    var value = reader.GetBoolean();
                    if (typeof(T) == typeof(bool)) return (Supports<T>) (object) new Supports<bool>(true, value);
                    return Supports.OfBoolean<T>(value);
                }

                if (typeof(T) == typeof(bool))
                {
                    using var _ = JsonDocument.ParseValue(ref reader);
                    return (Supports<T>) (object) new Supports<bool>(false, false);
                }
                var target = JsonSerializer.Deserialize<T>(ref reader, options);
                return new Supports<T>(!EqualityComparer<T>.Default.Equals(target!, default!), target!);
            }

            public override void Write(Utf8JsonWriter writer, Supports<T> value, JsonSerializerOptions options)
            {
                if (!value.IsSupported)
                {
                    writer.WriteNullValue();
                    return;
                }

                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }

    internal sealed class CompletionListConverter : JsonConverter<CompletionList>
    {
        public override CompletionList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Array)
            {
                var items = root.Deserialize<IEnumerable<CompletionItem>>(options) ?? Enumerable.Empty<CompletionItem>();
                return new CompletionList(items);
            }

            var listItems = root.TryGetProperty("items", out var itemsElement)
                ? itemsElement.Deserialize<IEnumerable<CompletionItem>>(options) ?? Enumerable.Empty<CompletionItem>()
                : Enumerable.Empty<CompletionItem>();
            return new CompletionList(listItems, root.TryGetProperty("isIncomplete", out var isIncomplete) && isIncomplete.GetBoolean()) {
                ItemDefaults = root.TryGetProperty("itemDefaults", out var itemDefaults)
                    ? itemDefaults.Deserialize<CompletionListItemDefaults>(options)
                    : null,
                ApplyKind = root.TryGetProperty("applyKind", out var applyKind)
                    ? applyKind.Deserialize<CompletionItemApplyKinds>(options)
                    : null,
            };
        }

        public override void Write(Utf8JsonWriter writer, CompletionList value, JsonSerializerOptions options)
        {
            if (!value.IsIncomplete && value.ItemDefaults is null && value.ApplyKind is null)
            {
                JsonSerializer.Serialize(writer, value.Items.ToArray(), options);
                return;
            }

            writer.WriteStartObject();
            writer.WriteBoolean("isIncomplete", value.IsIncomplete);
            writer.WritePropertyName("items");
            JsonSerializer.Serialize(writer, value.Items.ToArray(), options);
            if (value.ItemDefaults is not null)
            {
                writer.WritePropertyName("itemDefaults");
                JsonSerializer.Serialize(writer, value.ItemDefaults, options);
            }

            if (value.ApplyKind is not null)
            {
                writer.WritePropertyName("applyKind");
                JsonSerializer.Serialize(writer, value.ApplyKind, options);
            }

            writer.WriteEndObject();
        }
    }

    internal sealed class TypedCompletionListConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(CompletionList<>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var itemType = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter) Activator.CreateInstance(typeof(TypedCompletionListConverter<>).MakeGenericType(itemType))!;
        }

        private sealed class TypedCompletionListConverter<T> : JsonConverter<CompletionList<T>> where T : class?, IHandlerIdentity?
        {
            public override CompletionList<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var completionList = JsonSerializer.Deserialize<CompletionList>(ref reader, options);
                return CompletionList<T>.Create(completionList);
            }

            public override void Write(Utf8JsonWriter writer, CompletionList<T> value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, (CompletionList?) value, options);
            }
        }
    }

    internal sealed class StringOrInlayHintLabelPartsConverter : JsonConverter<StringOrInlayHintLabelParts>
    {
        public override StringOrInlayHintLabelParts Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String) return new StringOrInlayHintLabelParts(reader.GetString()!);
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var parts = JsonSerializer.Deserialize<Container<InlayHintLabelPart>>(ref reader, options) ?? new Container<InlayHintLabelPart>();
                return new StringOrInlayHintLabelParts(parts);
            }

            return new StringOrInlayHintLabelParts(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, StringOrInlayHintLabelParts value, JsonSerializerOptions options)
        {
            if (value.HasString) writer.WriteStringValue(value.String);
            else JsonSerializer.Serialize(writer, value.InlayHintLabelParts ?? Array.Empty<InlayHintLabelPart>(), options);
        }
    }

    internal sealed class RangeOrEditRangeConverter : JsonConverter<RangeOrEditRange>
    {
        public override RangeOrEditRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("insert", out _)) return new RangeOrEditRange(root.Deserialize<EditRange>(options)!);
            return new RangeOrEditRange(root.Deserialize<Range>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, RangeOrEditRange value, JsonSerializerOptions options)
        {
            if (value.IsRange) JsonSerializer.Serialize(writer, value.Range, options);
            else JsonSerializer.Serialize(writer, value.EditRange, options);
        }
    }

    internal sealed class InlineCompletionListConverter : JsonConverter<InlineCompletionList>
    {
        public override InlineCompletionList? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.ValueKind == JsonValueKind.Array)
            {
                return new InlineCompletionList(root.Deserialize<IEnumerable<InlineCompletionItem>>(options) ?? Array.Empty<InlineCompletionItem>());
            }

            var items = root.TryGetProperty("items", out var itemsElement)
                ? itemsElement.Deserialize<IEnumerable<InlineCompletionItem>>(options) ?? Array.Empty<InlineCompletionItem>()
                : Array.Empty<InlineCompletionItem>();
            return new InlineCompletionList(items);
        }

        public override void Write(Utf8JsonWriter writer, InlineCompletionList value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("items");
            JsonSerializer.Serialize(writer, value.Items.ToArray(), options);
            writer.WriteEndObject();
        }
    }

    internal sealed class StringOrStringValueConverter : JsonConverter<StringOrStringValue>
    {
        public override StringOrStringValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var value = JsonSerializer.Deserialize<StringValue>(ref reader, options)!;
                return new StringOrStringValue(value);
            }

            return new StringOrStringValue(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, StringOrStringValue value, JsonSerializerOptions options)
        {
            if (value.HasString) writer.WriteStringValue(value.String);
            else JsonSerializer.Serialize(writer, value.StringValue, options);
        }
    }

    internal sealed class InlineValueBaseConverter : JsonConverter<InlineValueBase>
    {
        public override InlineValueBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("text", out var text))
            {
                return new InlineValueText {
                    Range = root.GetProperty("range").Deserialize<Range>(options)!,
                    Text = text.GetString()!,
                };
            }

            if (root.TryGetProperty("variableName", out var variableName) || root.TryGetProperty("caseSensitiveLookup", out _))
            {
                return new InlineValueVariableLookup {
                    Range = root.GetProperty("range").Deserialize<Range>(options)!,
                    VariableName = variableName.ValueKind == JsonValueKind.String ? variableName.GetString() : null,
                    CaseSensitiveLookup = root.TryGetProperty("caseSensitiveLookup", out var csl) && csl.GetBoolean(),
                };
            }

            return new InlineValueEvaluatableExpression {
                Range = root.GetProperty("range").Deserialize<Range>(options)!,
                Expression = root.TryGetProperty("expression", out var expression) && expression.ValueKind == JsonValueKind.String
                    ? expression.GetString()
                    : null,
            };
        }

        public override void Write(Utf8JsonWriter writer, InlineValueBase value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }

    internal sealed class StringOrNotebookDocumentFilterConverter : JsonConverter<StringOrNotebookDocumentFilter>
    {
        public override StringOrNotebookDocumentFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var value = JsonSerializer.Deserialize<NotebookDocumentFilter>(ref reader, options)!;
                return new StringOrNotebookDocumentFilter(value);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new StringOrNotebookDocumentFilter(reader.GetString()!);
            }

            return new StringOrNotebookDocumentFilter(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, StringOrNotebookDocumentFilter value, JsonSerializerOptions options)
        {
            if (value.HasString) writer.WriteStringValue(value.String);
            else JsonSerializer.Serialize(writer, value.NotebookDocumentFilter, options);
        }
    }

    internal sealed class ContainerBaseConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert == typeof(Container<MarkedString>) || typeToConvert == typeof(LocationOrLocationLinks)) return false;
            return TryGetContainerElementType(typeToConvert, out _);
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (!TryGetContainerElementType(typeToConvert, out var elementType))
            {
                throw new NotSupportedException($"Cannot create container converter for {typeToConvert.FullName}.");
            }

            return (JsonConverter) Activator.CreateInstance(typeof(ContainerBaseConverter<,>).MakeGenericType(typeToConvert, elementType))!;
        }

        private static bool TryGetContainerElementType(Type type, out Type elementType)
        {
            var current = type;
            while (current is not null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(ContainerBase<>))
                {
                    elementType = current.GetGenericArguments()[0];
                    return true;
                }

                current = current.BaseType!;
            }

            elementType = typeof(object);
            return false;
        }

        private sealed class ContainerBaseConverter<TContainer, TItem> : JsonConverter<TContainer>
            where TContainer : class, IEnumerable<TItem>
        {
            public override TContainer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;

                TItem[] items;
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    items = JsonSerializer.Deserialize<TItem[]>(ref reader, options) ?? Array.Empty<TItem>();
                }
                else
                {
                    var item = JsonSerializer.Deserialize<TItem>(ref reader, options);
                    items = item is null ? Array.Empty<TItem>() : new[] { item };
                }

                return Create(items);
            }

            public override void Write(Utf8JsonWriter writer, TContainer value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value.ToArray(), options);
            }

            private static TContainer Create(IEnumerable<TItem> items)
            {
                if (typeof(TContainer) == typeof(Container<TItem>))
                {
                    return (TContainer) (object) new Container<TItem>(items);
                }

                var enumerableCtor = typeof(TContainer).GetConstructor(new[] { typeof(IEnumerable<TItem>) });
                if (enumerableCtor is not null)
                {
                    return (TContainer) enumerableCtor.Invoke(new object[] { items });
                }

                var array = items as TItem[] ?? items.ToArray();
                var arrayCtor = typeof(TContainer).GetConstructor(new[] { typeof(TItem[]) });
                if (arrayCtor is not null)
                {
                    return (TContainer) arrayCtor.Invoke(new object[] { array });
                }

                throw new NotSupportedException($"Type {typeof(TContainer).FullName} must expose an IEnumerable<{typeof(TItem).Name}> or {typeof(TItem).Name}[] constructor.");
            }
        }
    }

    internal sealed class StjDocumentUriConverter : JsonConverter<DocumentUri?>
    {
        public override DocumentUri? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("The JSON value must be a string.");

            try
            {
                var value = reader.GetString();
                return value is null ? null : DocumentUri.Parse(value);
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("Could not deserialize document uri", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, DocumentUri? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.ToString());
        }

        public override DocumentUri ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrWhiteSpace(value)) throw new JsonException("The JSON property name must be a non-empty document uri string.");
            return DocumentUri.Parse(value);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, DocumentUri value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString());
        }
    }

    internal sealed class StjDiagnosticCodeConverter : JsonConverter<DiagnosticCode>
    {
        public override DiagnosticCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new DiagnosticCode(reader.GetString()!);
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var l))
            {
                return new DiagnosticCode(l);
            }

            return default;
        }

        public override void Write(Utf8JsonWriter writer, DiagnosticCode value, JsonSerializerOptions options)
        {
            if (value.IsLong)
            {
                writer.WriteNumberValue(value.Long);
                return;
            }

            if (value.IsString)
            {
                writer.WriteStringValue(value.String);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjNullableDiagnosticCodeConverter : JsonConverter<DiagnosticCode?>
    {
        public override DiagnosticCode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if (reader.TokenType == JsonTokenType.String) return new DiagnosticCode(reader.GetString()!);
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var l)) return new DiagnosticCode(l);
            return null;
        }

        public override void Write(Utf8JsonWriter writer, DiagnosticCode? value, JsonSerializerOptions options)
        {
            if (!value.HasValue)
            {
                writer.WriteNullValue();
                return;
            }

            if (value.Value.IsLong)
            {
                writer.WriteNumberValue(value.Value.Long);
                return;
            }

            if (value.Value.IsString)
            {
                writer.WriteStringValue(value.Value.String);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjLocationOrLocationLinkConverter : JsonConverter<LocationOrLocationLink>
    {
        public override LocationOrLocationLink Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("uri", out _))
            {
                return new LocationOrLocationLink(root.Deserialize<Location>(options)!);
            }

            return new LocationOrLocationLink(root.Deserialize<LocationLink>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, LocationOrLocationLink value, JsonSerializerOptions options)
        {
            if (value.IsLocation)
            {
                JsonSerializer.Serialize(writer, value.Location, options);
                return;
            }

            if (value.IsLocationLink)
            {
                JsonSerializer.Serialize(writer, value.LocationLink, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjLocationOrLocationLinksConverter : JsonConverter<LocationOrLocationLinks>
    {
        public override LocationOrLocationLinks Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var values = JsonSerializer.Deserialize<IEnumerable<LocationOrLocationLink>>(ref reader, options) ?? Enumerable.Empty<LocationOrLocationLink>();
                return new LocationOrLocationLinks(values);
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var value = JsonSerializer.Deserialize<Location>(ref reader, options);
                return value is null ? new LocationOrLocationLinks() : new LocationOrLocationLinks(value);
            }

            return new LocationOrLocationLinks();
        }

        public override void Write(Utf8JsonWriter writer, LocationOrLocationLinks value, JsonSerializerOptions options)
        {
            var values = value.ToArray();
            if (values.Length == 1 && values[0].IsLocation)
            {
                JsonSerializer.Serialize(writer, values[0], options);
                return;
            }

            JsonSerializer.Serialize(writer, values, options);
        }
    }

    internal sealed class StjMarkedStringConverter : JsonConverter<MarkedString>
    {
        public override MarkedString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var document = JsonDocument.ParseValue(ref reader);
                var root = document.RootElement;
                var language = root.TryGetProperty("language", out var lang) && lang.ValueKind == JsonValueKind.String ? lang.GetString() : null;
                var value = root.TryGetProperty("value", out var text) && text.ValueKind == JsonValueKind.String ? text.GetString() : string.Empty;
                return new MarkedString(language, value ?? string.Empty);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new MarkedString(reader.GetString()!);
            }

            return new MarkedString(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, MarkedString value, JsonSerializerOptions options)
        {
            if (string.IsNullOrWhiteSpace(value.Language))
            {
                writer.WriteStringValue(value.Value);
                return;
            }

            writer.WriteStartObject();
            writer.WriteString("language", value.Language);
            writer.WriteString("value", value.Value);
            writer.WriteEndObject();
        }
    }

    internal sealed class StjMarkedStringCollectionConverter : JsonConverter<Container<MarkedString>>
    {
        public override Container<MarkedString> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var values = JsonSerializer.Deserialize<IEnumerable<MarkedString>>(ref reader, options) ?? Enumerable.Empty<MarkedString>();
                return new Container<MarkedString>(values);
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var value = JsonSerializer.Deserialize<MarkedString>(ref reader, options);
                return value is null ? new Container<MarkedString>() : new Container<MarkedString>(value);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new Container<MarkedString>(reader.GetString()!);
            }

            return new Container<MarkedString>();
        }

        public override void Write(Utf8JsonWriter writer, Container<MarkedString> value, JsonSerializerOptions options)
        {
            var values = value.ToArray();
            if (values.Length == 1)
            {
                JsonSerializer.Serialize(writer, values[0], options);
                return;
            }

            JsonSerializer.Serialize(writer, values, options);
        }
    }

    internal sealed class StjStringOrMarkupContentConverter : JsonConverter<StringOrMarkupContent>
    {
        public override StringOrMarkupContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var content = JsonSerializer.Deserialize<MarkupContent>(ref reader, options) ?? new MarkupContent();
                return new StringOrMarkupContent(content);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new StringOrMarkupContent(reader.GetString()!);
            }

            return new StringOrMarkupContent(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, StringOrMarkupContent value, JsonSerializerOptions options)
        {
            if (value.HasString)
            {
                writer.WriteStringValue(value.String);
                return;
            }

            JsonSerializer.Serialize(writer, value.MarkupContent, options);
        }
    }

    internal sealed class StjMarkedStringsOrMarkupContentConverter : JsonConverter<MarkedStringsOrMarkupContent>
    {
        public override MarkedStringsOrMarkupContent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var content = JsonSerializer.Deserialize<MarkupContent>(ref reader, options) ?? new MarkupContent();
                return new MarkedStringsOrMarkupContent(content);
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var strings = JsonSerializer.Deserialize<Container<MarkedString>>(ref reader, options) ?? new Container<MarkedString>();
                return new MarkedStringsOrMarkupContent(strings);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new MarkedStringsOrMarkupContent(reader.GetString()!);
            }

            return new MarkedStringsOrMarkupContent(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, MarkedStringsOrMarkupContent value, JsonSerializerOptions options)
        {
            if (value.HasMarkupContent)
            {
                JsonSerializer.Serialize(writer, value.MarkupContent, options);
                return;
            }

            JsonSerializer.Serialize(writer, value.MarkedStrings, options);
        }
    }

    internal sealed class StjTextDocumentSyncConverter : JsonConverter<TextDocumentSync?>
    {
        public override TextDocumentSync Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var i))
            {
                return new TextDocumentSync((TextDocumentSyncKind) i);
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return new TextDocumentSync(TextDocumentSyncKind.None);
            }

            var value = JsonSerializer.Deserialize<TextDocumentSyncOptions>(ref reader, options);
            return new TextDocumentSync(value!);
        }

        public override void Write(Utf8JsonWriter writer, TextDocumentSync? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNumberValue((int) TextDocumentSyncKind.None);
                return;
            }

            if (value.HasOptions)
            {
                JsonSerializer.Serialize(writer, value.Options, options);
                return;
            }

            if (value.HasKind)
            {
                writer.WriteNumberValue((int) value.Kind);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjBooleanNumberStringConverter : JsonConverter<BooleanNumberString>
    {
        public override BooleanNumberString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var i))
            {
                return new BooleanNumberString(i);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                return new BooleanNumberString(reader.GetString()!);
            }

            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                return new BooleanNumberString(reader.GetBoolean());
            }

            return new BooleanNumberString();
        }

        public override void Write(Utf8JsonWriter writer, BooleanNumberString value, JsonSerializerOptions options)
        {
            if (value.IsBool)
            {
                writer.WriteBooleanValue(value.Bool);
                return;
            }

            if (value.IsInteger)
            {
                writer.WriteNumberValue(value.Integer);
                return;
            }

            if (value.IsString)
            {
                writer.WriteStringValue(value.String);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjBooleanStringConverter : JsonConverter<BooleanString>
    {
        public override BooleanString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new BooleanString(reader.GetString()!);
            }

            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
            {
                return new BooleanString(reader.GetBoolean());
            }

            return new BooleanString();
        }

        public override void Write(Utf8JsonWriter writer, BooleanString value, JsonSerializerOptions options)
        {
            if (value.IsBool)
            {
                writer.WriteBooleanValue(value.Bool);
                return;
            }

            if (value.IsString)
            {
                writer.WriteStringValue(value.String);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjBooleanOrConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(BooleanOr<>);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var target = typeToConvert.GetGenericArguments()[0];
            return (JsonConverter) Activator.CreateInstance(typeof(StjBooleanOrConverter<>).MakeGenericType(target))!;
        }

        private sealed class StjBooleanOrConverter<T> : JsonConverter<BooleanOr<T>> where T : class?
        {
            public override BooleanOr<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                {
                    return new BooleanOr<T>(reader.GetBoolean());
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var value = JsonSerializer.Deserialize<T>(ref reader, options);
                    return value is null ? new BooleanOr<T>(false) : new BooleanOr<T>(value);
                }

                return new BooleanOr<T>(false);
            }

            public override void Write(Utf8JsonWriter writer, BooleanOr<T> value, JsonSerializerOptions options)
            {
                if (value.IsBool)
                {
                    writer.WriteBooleanValue(value.Bool);
                    return;
                }

                if (value.IsValue)
                {
                    JsonSerializer.Serialize(writer, value.Value, options);
                    return;
                }

                writer.WriteNullValue();
            }
        }
    }

    internal sealed class StjProgressTokenConverter : JsonConverter<ProgressToken?>
    {
        public override ProgressToken? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt64(out var l)) return new ProgressToken(l);
            if (reader.TokenType == JsonTokenType.String)
            {
                var value = reader.GetString();
                return string.IsNullOrWhiteSpace(value) ? null : new ProgressToken(value);
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, ProgressToken? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            if (value.IsLong)
            {
                writer.WriteNumberValue(value.Long);
                return;
            }

            if (value.IsString)
            {
                writer.WriteStringValue(value.String);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjCommandOrCodeActionConverter : JsonConverter<CommandOrCodeAction>
    {
        public override CommandOrCodeAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("command", out var command) && command.ValueKind == JsonValueKind.String)
            {
                return new CommandOrCodeAction(root.Deserialize<Command>(options)!);
            }

            return new CommandOrCodeAction(root.Deserialize<CodeAction>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, CommandOrCodeAction value, JsonSerializerOptions options)
        {
            if (value.IsCodeAction)
            {
                JsonSerializer.Serialize(writer, value.CodeAction, options);
                return;
            }

            if (value.IsCommand)
            {
                JsonSerializer.Serialize(writer, value.Command, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjSemanticTokensFullOrDeltaConverter : JsonConverter<SemanticTokensFullOrDelta>
    {
        public override SemanticTokensFullOrDelta Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("data", out _))
            {
                return new SemanticTokensFullOrDelta(root.Deserialize<SemanticTokens>(options)!);
            }

            return new SemanticTokensFullOrDelta(root.Deserialize<SemanticTokensDelta>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, SemanticTokensFullOrDelta value, JsonSerializerOptions options)
        {
            if (value.IsFull)
            {
                JsonSerializer.Serialize(writer, value.Full, options);
                return;
            }

            if (value.IsDelta)
            {
                JsonSerializer.Serialize(writer, value.Delta, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjSemanticTokensFullOrDeltaPartialResultConverter : JsonConverter<SemanticTokensFullOrDeltaPartialResult>
    {
        public override SemanticTokensFullOrDeltaPartialResult Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("data", out _))
            {
                return new SemanticTokensFullOrDeltaPartialResult(root.Deserialize<SemanticTokensPartialResult>(options)!);
            }

            return new SemanticTokensFullOrDeltaPartialResult(root.Deserialize<SemanticTokensDeltaPartialResult>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, SemanticTokensFullOrDeltaPartialResult value, JsonSerializerOptions options)
        {
            if (value.IsDelta)
            {
                JsonSerializer.Serialize(writer, value.Delta, options);
                return;
            }

            if (value.IsFull)
            {
                JsonSerializer.Serialize(writer, value.Full, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjSymbolInformationOrDocumentSymbolConverter : JsonConverter<SymbolInformationOrDocumentSymbol>
    {
        public override SymbolInformationOrDocumentSymbol Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("location", out _))
            {
                return new SymbolInformationOrDocumentSymbol(root.Deserialize<SymbolInformation>(options)!);
            }

            return new SymbolInformationOrDocumentSymbol(root.Deserialize<DocumentSymbol>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, SymbolInformationOrDocumentSymbol value, JsonSerializerOptions options)
        {
            if (value.IsDocumentSymbolInformation)
            {
                JsonSerializer.Serialize(writer, value.SymbolInformation, options);
                return;
            }

            if (value.IsDocumentSymbol)
            {
                JsonSerializer.Serialize(writer, value.DocumentSymbol, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjWorkspaceEditDocumentChangeConverter : JsonConverter<WorkspaceEditDocumentChange>
    {
        public override WorkspaceEditDocumentChange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("kind", out var kind) && kind.ValueKind == JsonValueKind.String)
            {
                switch (kind.GetString())
                {
                    case "create":
                        return new WorkspaceEditDocumentChange(root.Deserialize<CreateFile>(options)!);
                    case "rename":
                        return new WorkspaceEditDocumentChange(root.Deserialize<RenameFile>(options)!);
                    case "delete":
                        return new WorkspaceEditDocumentChange(root.Deserialize<DeleteFile>(options)!);
                    default:
                        throw new NotSupportedException("Object with " + kind.GetString() + " is not supported");
                }
            }

            return new WorkspaceEditDocumentChange(root.Deserialize<TextDocumentEdit>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, WorkspaceEditDocumentChange value, JsonSerializerOptions options)
        {
            if (value.IsCreateFile)
            {
                JsonSerializer.Serialize(writer, value.CreateFile, options);
                return;
            }

            if (value.IsDeleteFile)
            {
                JsonSerializer.Serialize(writer, value.DeleteFile, options);
                return;
            }

            if (value.IsRenameFile)
            {
                JsonSerializer.Serialize(writer, value.RenameFile, options);
                return;
            }

            if (value.IsTextDocumentEdit)
            {
                JsonSerializer.Serialize(writer, value.TextDocumentEdit, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjParameterInformationLabelConverter : JsonConverter<ParameterInformationLabel>
    {
        public override ParameterInformationLabel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new ParameterInformationLabel(reader.GetString()!);
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var values = JsonSerializer.Deserialize<int[]>(ref reader, options) ?? Array.Empty<int>();
                if (values.Length < 2) throw new JsonException("ParameterInformationLabel range must contain at least 2 values.");
                return new ParameterInformationLabel((values[0], values[1]));
            }

            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, ParameterInformationLabel value, JsonSerializerOptions options)
        {
            if (value.IsLabel)
            {
                writer.WriteStringValue(value.Label);
                return;
            }

            if (value.IsRange)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(value.Range.start);
                writer.WriteNumberValue(value.Range.end);
                writer.WriteEndArray();
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjValueTupleLongLongConverter : JsonConverter<(long, long)>
    {
        public override (long, long) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var values = JsonSerializer.Deserialize<long[]>(ref reader, options) ?? Array.Empty<long>();
            if (values.Length < 2) throw new JsonException("Expected an array with two long values.");
            return (values[0], values[1]);
        }

        public override void Write(Utf8JsonWriter writer, (long, long) value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            writer.WriteNumberValue(value.Item1);
            writer.WriteNumberValue(value.Item2);
            writer.WriteEndArray();
        }
    }

    internal sealed class StjRangeOrPlaceholderRangeConverter : JsonConverter<RangeOrPlaceholderRange?>
    {
        public override RangeOrPlaceholderRange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) return null;

            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("placeholder", out _))
            {
                return new RangeOrPlaceholderRange(root.Deserialize<PlaceholderRange>(options)!);
            }

            if (root.TryGetProperty("defaultBehavior", out _))
            {
                return new RangeOrPlaceholderRange(root.Deserialize<RenameDefaultBehavior>(options)!);
            }

            return new RangeOrPlaceholderRange(root.Deserialize<Range>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, RangeOrPlaceholderRange? value, JsonSerializerOptions options)
        {
            if (value?.IsRange == true)
            {
                JsonSerializer.Serialize(writer, value.Range, options);
                return;
            }

            if (value?.IsPlaceholderRange == true)
            {
                JsonSerializer.Serialize(writer, value.PlaceholderRange, options);
                return;
            }

            if (value?.IsDefaultBehavior == true)
            {
                JsonSerializer.Serialize(writer, value.DefaultBehavior, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjChangeAnnotationIdentifierConverter : JsonConverter<ChangeAnnotationIdentifier?>
    {
        public override ChangeAnnotationIdentifier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("The JSON value must be a string.");

            try
            {
                return new ChangeAnnotationIdentifier { Identifier = reader.GetString()! };
            }
            catch (ArgumentException ex)
            {
                throw new JsonException("Could not deserialize change annotation identifier", ex);
            }
        }

        public override void Write(Utf8JsonWriter writer, ChangeAnnotationIdentifier? value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.Identifier);
        }
    }

    internal sealed class StjAggregateCompletionListConverter : JsonConverter<AggregateResponse<CompletionList>>
    {
        public override AggregateResponse<CompletionList> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotImplementedException();

        public override void Write(Utf8JsonWriter writer, AggregateResponse<CompletionList> value, JsonSerializerOptions options)
        {
            var values = value.Items.ToArray();
            if (!values.Any(z => z.IsIncomplete))
            {
                writer.WriteStartArray();
                foreach (var item in value.Items)
                {
                    foreach (var completion in item)
                    {
                        JsonSerializer.Serialize(writer, completion, options);
                    }
                }

                writer.WriteEndArray();
                return;
            }

            writer.WriteStartObject();
            writer.WriteBoolean("isIncomplete", true);
            writer.WritePropertyName("items");
            writer.WriteStartArray();
            foreach (var item in value.Items)
            {
                foreach (var completion in item)
                {
                    JsonSerializer.Serialize(writer, completion, options);
                }
            }

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    internal sealed class StjLspAnyConverter : JsonConverter<LSPAny>
    {
        private static readonly JsonElement NullElement = JsonDocument.Parse("null").RootElement.Clone();

        public override LSPAny Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return new LSPAny(NullElement);
            }

            using var document = JsonDocument.ParseValue(ref reader);
            return new LSPAny(document.RootElement.Clone());
        }

        public override void Write(Utf8JsonWriter writer, LSPAny value, JsonSerializerOptions options)
        {
            if (value.Value.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            {
                writer.WriteNullValue();
                return;
            }

            value.Value.WriteTo(writer);
        }
    }

    internal sealed class StjObjectPrimitiveOrElementConverter : JsonConverter<object?>
    {
        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    return null;
                case JsonTokenType.True:
                case JsonTokenType.False:
                    return reader.GetBoolean();
                case JsonTokenType.String:
                    return reader.GetString();
                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var l)) return l;
                    return reader.GetDouble();
                case JsonTokenType.StartArray:
                case JsonTokenType.StartObject:
                    using (var document = JsonDocument.ParseValue(ref reader))
                    {
                        return document.RootElement.Clone();
                    }
                default:
                    throw new JsonException($"Unsupported token {reader.TokenType} for object.");
            }
        }

        public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    return;
                case JsonElement element:
                    element.WriteTo(writer);
                    return;
                case string s:
                    writer.WriteStringValue(s);
                    return;
                case bool b:
                    writer.WriteBooleanValue(b);
                    return;
                case byte b8:
                    writer.WriteNumberValue(b8);
                    return;
                case sbyte sb8:
                    writer.WriteNumberValue(sb8);
                    return;
                case short s16:
                    writer.WriteNumberValue(s16);
                    return;
                case ushort us16:
                    writer.WriteNumberValue(us16);
                    return;
                case int i32:
                    writer.WriteNumberValue(i32);
                    return;
                case uint ui32:
                    writer.WriteNumberValue(ui32);
                    return;
                case long i64:
                    writer.WriteNumberValue(i64);
                    return;
                case ulong ui64:
                    writer.WriteNumberValue(ui64);
                    return;
                case float f:
                    writer.WriteNumberValue(f);
                    return;
                case double d:
                    writer.WriteNumberValue(d);
                    return;
                case decimal m:
                    writer.WriteNumberValue(m);
                    return;
                default:
                    if (value.GetType() == typeof(object))
                    {
                        writer.WriteStartObject();
                        writer.WriteEndObject();
                        return;
                    }

                    JsonSerializer.Serialize(writer, value, value.GetType(), options);
                    return;
            }
        }
    }

    internal sealed class StjJTokenConverter : JsonConverter<JToken>
    {
        public override JToken? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            return JToken.Parse(document.RootElement.GetRawText());
        }

        public override void Write(Utf8JsonWriter writer, JToken value, JsonSerializerOptions options)
        {
            using var document = JsonDocument.Parse(value.ToString(Newtonsoft.Json.Formatting.None));
            document.RootElement.WriteTo(writer);
        }
    }

    internal sealed class StjJObjectConverter : JsonConverter<JObject>
    {
        public override JObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            return JObject.Parse(document.RootElement.GetRawText());
        }

        public override void Write(Utf8JsonWriter writer, JObject value, JsonSerializerOptions options)
        {
            using var document = JsonDocument.Parse(value.ToString(Newtonsoft.Json.Formatting.None));
            document.RootElement.WriteTo(writer);
        }
    }

    internal sealed class StjOptionalBooleanConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return false;
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False) return reader.GetBoolean();
            throw new JsonException("Expected boolean or null.");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    internal sealed class StjDocumentDiagnosticReportConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeof(DocumentDiagnosticReport).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter) Activator.CreateInstance(typeof(StjDocumentDiagnosticReportConverter<>).MakeGenericType(typeToConvert))!;
        }

        private sealed class StjDocumentDiagnosticReportConverter<T> : JsonConverter<T> where T : DocumentDiagnosticReport
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = new StjDocumentDiagnosticReportConverter().Read(ref reader, typeof(DocumentDiagnosticReport), options);
                return (T?) value;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                DiagnosticReportStjConverter.WriteDocumentDiagnosticReport(writer, value, options);
            }
        }
    }

    internal sealed class StjDocumentDiagnosticReportConverter : JsonConverter<DocumentDiagnosticReport>
    {
        public override DocumentDiagnosticReport? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            return DiagnosticReportStjConverter.ReadDocumentDiagnosticReport(document.RootElement, options);
        }

        public override void Write(Utf8JsonWriter writer, DocumentDiagnosticReport value, JsonSerializerOptions options)
        {
            DiagnosticReportStjConverter.WriteDocumentDiagnosticReport(writer, value, options);
        }
    }

    internal sealed class StjRelatedDocumentDiagnosticReportConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeof(RelatedDocumentDiagnosticReport).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter) Activator.CreateInstance(typeof(StjRelatedDocumentDiagnosticReportConverter<>).MakeGenericType(typeToConvert))!;
        }

        private sealed class StjRelatedDocumentDiagnosticReportConverter<T> : JsonConverter<T> where T : RelatedDocumentDiagnosticReport
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = new StjRelatedDocumentDiagnosticReportConverter().Read(ref reader, typeof(RelatedDocumentDiagnosticReport), options);
                return (T?) value;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                DiagnosticReportStjConverter.WriteRelatedDocumentDiagnosticReport(writer, value, options);
            }
        }
    }

    internal sealed class StjRelatedDocumentDiagnosticReportConverter : JsonConverter<RelatedDocumentDiagnosticReport>
    {
        public override RelatedDocumentDiagnosticReport? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            return DiagnosticReportStjConverter.ReadRelatedDocumentDiagnosticReport(document.RootElement, options);
        }

        public override void Write(Utf8JsonWriter writer, RelatedDocumentDiagnosticReport value, JsonSerializerOptions options)
        {
            DiagnosticReportStjConverter.WriteRelatedDocumentDiagnosticReport(writer, value, options);
        }
    }

    internal sealed class StjWorkspaceDocumentDiagnosticReportConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeof(WorkspaceDocumentDiagnosticReport).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter) Activator.CreateInstance(typeof(StjWorkspaceDocumentDiagnosticReportConverter<>).MakeGenericType(typeToConvert))!;
        }

        private sealed class StjWorkspaceDocumentDiagnosticReportConverter<T> : JsonConverter<T> where T : WorkspaceDocumentDiagnosticReport
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = new StjWorkspaceDocumentDiagnosticReportConverter().Read(ref reader, typeof(WorkspaceDocumentDiagnosticReport), options);
                return (T?) value;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                DiagnosticReportStjConverter.WriteWorkspaceDocumentDiagnosticReport(writer, value, options);
            }
        }
    }

    internal sealed class StjWorkspaceDocumentDiagnosticReportConverter : JsonConverter<WorkspaceDocumentDiagnosticReport>
    {
        public override WorkspaceDocumentDiagnosticReport? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            using var document = JsonDocument.ParseValue(ref reader);
            return DiagnosticReportStjConverter.ReadWorkspaceDocumentDiagnosticReport(document.RootElement, options);
        }

        public override void Write(Utf8JsonWriter writer, WorkspaceDocumentDiagnosticReport value, JsonSerializerOptions options)
        {
            DiagnosticReportStjConverter.WriteWorkspaceDocumentDiagnosticReport(writer, value, options);
        }
    }

    internal static class DiagnosticReportStjConverter
    {
        public static DocumentDiagnosticReport ReadDocumentDiagnosticReport(JsonElement element, JsonSerializerOptions options)
        {
            var kind = ReadKind(element);
            if (kind == DocumentDiagnosticReportKind.Full)
            {
                return new FullDocumentDiagnosticReport {
                    ResultId = ReadOptionalString(element, "resultId"),
                    Items = ReadItems(element, options)
                };
            }

            if (kind == DocumentDiagnosticReportKind.Unchanged)
            {
                return new UnchangedDocumentDiagnosticReport {
                    ResultId = ReadRequiredString(element, "resultId")
                };
            }

            throw new JsonException($"Unknown diagnostic report kind '{kind}'.");
        }

        public static RelatedDocumentDiagnosticReport ReadRelatedDocumentDiagnosticReport(JsonElement element, JsonSerializerOptions options)
        {
            var kind = ReadKind(element);
            if (kind == DocumentDiagnosticReportKind.Full)
            {
                return new RelatedFullDocumentDiagnosticReport {
                    ResultId = ReadOptionalString(element, "resultId"),
                    Items = ReadItems(element, options),
                    RelatedDocuments = ReadRelatedDocuments(element, options)
                };
            }

            if (kind == DocumentDiagnosticReportKind.Unchanged)
            {
                return new RelatedUnchangedDocumentDiagnosticReport {
                    ResultId = ReadRequiredString(element, "resultId"),
                    RelatedDocuments = ReadRelatedDocuments(element, options)
                };
            }

            throw new JsonException($"Unknown diagnostic report kind '{kind}'.");
        }

        public static WorkspaceDocumentDiagnosticReport ReadWorkspaceDocumentDiagnosticReport(JsonElement element, JsonSerializerOptions options)
        {
            var kind = ReadKind(element);
            var uri = ReadRequired<DocumentUri>(element, "uri", options);
            var version = ReadNullableInt32(element, "version");

            if (kind == DocumentDiagnosticReportKind.Full)
            {
                return new WorkspaceFullDocumentDiagnosticReport {
                    Uri = uri,
                    Version = version,
                    ResultId = ReadOptionalString(element, "resultId"),
                    Items = ReadItems(element, options)
                };
            }

            if (kind == DocumentDiagnosticReportKind.Unchanged)
            {
                return new WorkspaceUnchangedDocumentDiagnosticReport {
                    Uri = uri,
                    Version = version,
                    ResultId = ReadRequiredString(element, "resultId")
                };
            }

            throw new JsonException($"Unknown diagnostic report kind '{kind}'.");
        }

        public static void WriteDocumentDiagnosticReport(Utf8JsonWriter writer, DocumentDiagnosticReport? value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    return;
                case FullDocumentDiagnosticReport full:
                    WriteStart(writer, full.Kind);
                    WriteOptionalString(writer, "resultId", full.ResultId);
                    writer.WritePropertyName("items");
                    JsonSerializer.Serialize(writer, full.Items, options);
                    writer.WriteEndObject();
                    return;
                case UnchangedDocumentDiagnosticReport unchanged:
                    WriteStart(writer, unchanged.Kind);
                    writer.WriteString("resultId", unchanged.ResultId);
                    writer.WriteEndObject();
                    return;
                default:
                    throw new JsonException($"Unknown diagnostic report type {value.GetType().FullName}.");
            }
        }

        public static void WriteRelatedDocumentDiagnosticReport(Utf8JsonWriter writer, RelatedDocumentDiagnosticReport? value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    return;
                case RelatedFullDocumentDiagnosticReport full:
                    WriteStart(writer, full.Kind);
                    WriteOptionalString(writer, "resultId", full.ResultId);
                    writer.WritePropertyName("items");
                    JsonSerializer.Serialize(writer, full.Items, options);
                    WriteRelatedDocuments(writer, full.RelatedDocuments, options);
                    writer.WriteEndObject();
                    return;
                case RelatedUnchangedDocumentDiagnosticReport unchanged:
                    WriteStart(writer, unchanged.Kind);
                    writer.WriteString("resultId", unchanged.ResultId);
                    WriteRelatedDocuments(writer, unchanged.RelatedDocuments, options);
                    writer.WriteEndObject();
                    return;
                default:
                    throw new JsonException($"Unknown related diagnostic report type {value.GetType().FullName}.");
            }
        }

        public static void WriteWorkspaceDocumentDiagnosticReport(Utf8JsonWriter writer, WorkspaceDocumentDiagnosticReport? value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    writer.WriteNullValue();
                    return;
                case WorkspaceFullDocumentDiagnosticReport full:
                    WriteStart(writer, full.Kind);
                    writer.WritePropertyName("uri");
                    JsonSerializer.Serialize(writer, full.Uri, options);
                    writer.WritePropertyName("version");
                    JsonSerializer.Serialize(writer, full.Version, options);
                    WriteOptionalString(writer, "resultId", full.ResultId);
                    writer.WritePropertyName("items");
                    JsonSerializer.Serialize(writer, full.Items, options);
                    writer.WriteEndObject();
                    return;
                case WorkspaceUnchangedDocumentDiagnosticReport unchanged:
                    WriteStart(writer, unchanged.Kind);
                    writer.WritePropertyName("uri");
                    JsonSerializer.Serialize(writer, unchanged.Uri, options);
                    writer.WritePropertyName("version");
                    JsonSerializer.Serialize(writer, unchanged.Version, options);
                    writer.WriteString("resultId", unchanged.ResultId);
                    writer.WriteEndObject();
                    return;
                default:
                    throw new JsonException($"Unknown workspace diagnostic report type {value.GetType().FullName}.");
            }
        }

        private static void WriteStart(Utf8JsonWriter writer, DocumentDiagnosticReportKind kind)
        {
            writer.WriteStartObject();
            writer.WriteString("kind", kind.ToString());
        }

        private static void WriteOptionalString(Utf8JsonWriter writer, string name, string? value)
        {
            if (value is not null)
            {
                writer.WriteString(name, value);
            }
        }

        private static void WriteRelatedDocuments(
            Utf8JsonWriter writer,
            ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>? relatedDocuments,
            JsonSerializerOptions options
        )
        {
            if (relatedDocuments is null || relatedDocuments.Count == 0) return;

            writer.WritePropertyName("relatedDocuments");
            writer.WriteStartObject();
            foreach (var item in relatedDocuments)
            {
                writer.WritePropertyName(item.Key.ToString());
                WriteDocumentDiagnosticReport(writer, item.Value, options);
            }

            writer.WriteEndObject();
        }

        private static DocumentDiagnosticReportKind ReadKind(JsonElement element)
        {
            if (!element.TryGetProperty("kind", out var kindElement) || kindElement.ValueKind != JsonValueKind.String)
            {
                throw new JsonException("Diagnostic report is missing a kind.");
            }

            return new DocumentDiagnosticReportKind(kindElement.GetString()!);
        }

        private static string ReadRequiredString(JsonElement element, string name)
        {
            if (!element.TryGetProperty(name, out var value) || value.ValueKind != JsonValueKind.String)
            {
                throw new JsonException($"Diagnostic report is missing required property '{name}'.");
            }

            return value.GetString()!;
        }

        private static string? ReadOptionalString(JsonElement element, string name)
        {
            if (!element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null) return null;
            return value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        }

        private static int? ReadNullableInt32(JsonElement element, string name)
        {
            if (!element.TryGetProperty(name, out var value) || value.ValueKind == JsonValueKind.Null) return null;
            return value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var i) ? i : null;
        }

        private static Container<Diagnostic> ReadItems(JsonElement element, JsonSerializerOptions options)
        {
            if (!element.TryGetProperty("items", out var items)) return new Container<Diagnostic>();
            return items.Deserialize<Container<Diagnostic>>(options) ?? new Container<Diagnostic>();
        }

        private static ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>? ReadRelatedDocuments(JsonElement element, JsonSerializerOptions options)
        {
            if (!element.TryGetProperty("relatedDocuments", out var related) || related.ValueKind != JsonValueKind.Object)
            {
                return null;
            }

            var builder = ImmutableDictionary<DocumentUri, DocumentDiagnosticReport>.Empty.ToBuilder();
            foreach (var property in related.EnumerateObject())
            {
                builder.Add(DocumentUri.Parse(property.Name), ReadDocumentDiagnosticReport(property.Value, options));
            }

            return builder.ToImmutable();
        }

        private static T ReadRequired<T>(JsonElement element, string name, JsonSerializerOptions options)
        {
            if (!element.TryGetProperty(name, out var value))
            {
                throw new JsonException($"Diagnostic report is missing required property '{name}'.");
            }

            return value.Deserialize<T>(options) ?? throw new JsonException($"Diagnostic report property '{name}' cannot be null.");
        }
    }

    internal sealed class StjRpcErrorConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeof(RpcError).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return (JsonConverter) Activator.CreateInstance(typeof(StjRpcErrorConverter<>).MakeGenericType(typeToConvert))!;
        }

        private sealed class StjRpcErrorConverter<T> : JsonConverter<T> where T : RpcError
        {
            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var value = new StjRpcErrorConverter().Read(ref reader, typeof(RpcError), options);
                return value as T;
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                StjRpcErrorConverter.WriteRpcError(writer, value, options);
            }
        }
    }

    internal sealed class StjRpcErrorConverter : JsonConverter<RpcError>
    {
        public override RpcError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;

            object? id = null;
            if (root.TryGetProperty("id", out var idElement))
            {
                id = idElement.ValueKind switch
                {
                    JsonValueKind.String => idElement.GetString(),
                    JsonValueKind.Number when idElement.TryGetInt64(out var l) => l,
                    _ => null
                };
            }

            ErrorMessage? error = null;
            if (root.TryGetProperty("error", out var errorElement) && errorElement.ValueKind != JsonValueKind.Null)
            {
                if (!errorElement.TryGetProperty("code", out var codeElement) || !codeElement.TryGetInt32(out var code))
                {
                    throw new JsonException("Rpc error payload is missing required property 'code'.");
                }

                if (!errorElement.TryGetProperty("message", out var messageElement) || messageElement.ValueKind != JsonValueKind.String)
                {
                    throw new JsonException("Rpc error payload is missing required property 'message'.");
                }

                if (errorElement.TryGetProperty("data", out var dataElement) && dataElement.ValueKind != JsonValueKind.Null)
                {
                    var data = JsonSerializer.Deserialize<object>(dataElement.GetRawText(), options);
                    error = new ErrorMessage(code, messageElement.GetString()!, data!);
                }
                else
                {
                    error = new ErrorMessage(code, messageElement.GetString()!);
                }
            }

            return new RpcError(id, error);
        }

        public override void Write(Utf8JsonWriter writer, RpcError value, JsonSerializerOptions options)
            => WriteRpcError(writer, value, options);

        internal static void WriteRpcError(Utf8JsonWriter writer, RpcError value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("jsonrpc", "2.0");
            if (value.Id is not null)
            {
                writer.WritePropertyName("id");
                JsonSerializer.Serialize(writer, value.Id, options);
            }

            writer.WritePropertyName("error");
            if (value.Error is null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteNumber("code", value.Error.Code);
                if (value.Error.Data is not null)
                {
                    writer.WritePropertyName("data");
                    JsonSerializer.Serialize(writer, value.Error.Data, options);
                }

                writer.WriteString("message", value.Error.Message);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }

    internal sealed class StjAnnotatedTextEditConverter : JsonConverter<AnnotatedTextEdit>
    {
        public override AnnotatedTextEdit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => (AnnotatedTextEdit) JsonSerializer.Deserialize<TextEdit>(ref reader, options)!;

        public override void Write(Utf8JsonWriter writer, AnnotatedTextEdit value, JsonSerializerOptions options)
        {
            new StjTextEditConverter().Write(writer, value, options);
        }
    }

    internal sealed class StjSnippetTextEditConverter : JsonConverter<SnippetTextEdit>
    {
        public override SnippetTextEdit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => (SnippetTextEdit) JsonSerializer.Deserialize<TextEdit>(ref reader, options)!;

        public override void Write(Utf8JsonWriter writer, SnippetTextEdit value, JsonSerializerOptions options)
        {
            new StjTextEditConverter().Write(writer, value, options);
        }
    }

    internal sealed class StjTextEditConverter : JsonConverter<TextEdit>
    {
        public override TextEdit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;

            TextEdit edit;
            if (root.TryGetProperty("snippet", out var snippet) && snippet.ValueKind == JsonValueKind.Object)
            {
                edit = new SnippetTextEdit {
                    Snippet = snippet.Deserialize<StringValue>(options)!,
                    AnnotationId = root.TryGetProperty("annotationId", out var annotationId)
                        ? annotationId.Deserialize<ChangeAnnotationIdentifier>(options)
                        : null
                };
            }
            else if (root.TryGetProperty("annotationId", out var annotation) && annotation.ValueKind == JsonValueKind.String)
            {
                edit = new AnnotatedTextEdit {
                    AnnotationId = annotation.Deserialize<ChangeAnnotationIdentifier>(options)!
                };
            }
            else
            {
                edit = new TextEdit();
            }

            if (root.TryGetProperty("range", out var range) && range.ValueKind == JsonValueKind.Object)
            {
                edit = edit with { Range = range.Deserialize<Range>(options)! };
            }

            if (root.TryGetProperty("newText", out var newText) && newText.ValueKind == JsonValueKind.String)
            {
                edit = edit with { NewText = newText.GetString()! };
            }

            return edit;
        }

        public override void Write(Utf8JsonWriter writer, TextEdit value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("range");
            JsonSerializer.Serialize(writer, value.Range, options);

            if (value is SnippetTextEdit snippetTextEdit)
            {
                writer.WritePropertyName("snippet");
                JsonSerializer.Serialize(writer, snippetTextEdit.Snippet, options);
                if (snippetTextEdit.AnnotationId is { })
                {
                    writer.WritePropertyName("annotationId");
                    JsonSerializer.Serialize(writer, snippetTextEdit.AnnotationId, options);
                }

                writer.WriteEndObject();
                return;
            }

            writer.WritePropertyName("newText");
            JsonSerializer.Serialize(writer, value.NewText, options);
            if (value is AnnotatedTextEdit annotatedTextEdit)
            {
                writer.WritePropertyName("annotationId");
                JsonSerializer.Serialize(writer, annotatedTextEdit.AnnotationId, options);
            }

            writer.WriteEndObject();
        }
    }

    internal sealed class StjTextEditOrInsertReplaceEditConverter : JsonConverter<TextEditOrInsertReplaceEdit>
    {
        public override TextEditOrInsertReplaceEdit Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("insert", out var insert) && insert.ValueKind == JsonValueKind.Object)
            {
                return new TextEditOrInsertReplaceEdit(root.Deserialize<InsertReplaceEdit>(options)!);
            }

            return new TextEditOrInsertReplaceEdit(root.Deserialize<TextEdit>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, TextEditOrInsertReplaceEdit value, JsonSerializerOptions options)
        {
            if (value.IsTextEdit)
            {
                JsonSerializer.Serialize(writer, value.TextEdit, options);
                return;
            }

            if (value.IsInsertReplaceEdit)
            {
                JsonSerializer.Serialize(writer, value.InsertReplaceEdit, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class StjWorkspaceFolderOrUriConverter : JsonConverter<WorkspaceFolderOrUri>
    {
        public override WorkspaceFolderOrUri Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new WorkspaceFolderOrUri(reader.GetString()!);
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var document = JsonDocument.ParseValue(ref reader);
                var root = document.RootElement;
                if (root.TryGetProperty("name", out _))
                {
                    return new WorkspaceFolderOrUri(root.Deserialize<WorkspaceFolder>(options)!);
                }

                if (root.TryGetProperty("uri", out var uri) && uri.ValueKind == JsonValueKind.String)
                {
                    return new WorkspaceFolderOrUri(DocumentUri.Parse(uri.GetString()!));
                }

                return new WorkspaceFolderOrUri(root.Deserialize<DocumentUri>(options)!);
            }

            return new WorkspaceFolderOrUri(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, WorkspaceFolderOrUri value, JsonSerializerOptions options)
        {
            if (value.HasWorkspaceFolder)
            {
                JsonSerializer.Serialize(writer, value.WorkspaceFolder, options);
                return;
            }

            JsonSerializer.Serialize(writer, value.Uri, options);
        }
    }

    internal sealed class StjGlobPatternConverter : JsonConverter<GlobPattern>
    {
        public override GlobPattern Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new GlobPattern(reader.GetString()!);
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                return new GlobPattern(JsonSerializer.Deserialize<RelativePattern>(ref reader, options)!);
            }

            return new GlobPattern(string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, GlobPattern value, JsonSerializerOptions options)
        {
            if (value.HasPattern)
            {
                writer.WriteStringValue(value.Pattern);
                return;
            }

            JsonSerializer.Serialize(writer, value.RelativePattern, options);
        }
    }

    internal sealed class StjLocationOrFileLocationConverter : JsonConverter<LocationOrFileLocation>
    {
        public override LocationOrFileLocation Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;
            if (root.TryGetProperty("range", out _))
            {
                return new LocationOrFileLocation(root.Deserialize<Location>(options)!);
            }

            return new LocationOrFileLocation(root.Deserialize<FileLocation>(options)!);
        }

        public override void Write(Utf8JsonWriter writer, LocationOrFileLocation value, JsonSerializerOptions options)
        {
            if (value.IsLocation)
            {
                JsonSerializer.Serialize(writer, value.Location, options);
                return;
            }

            if (value.IsFileLocation)
            {
                JsonSerializer.Serialize(writer, value.FileLocation, options);
                return;
            }

            writer.WriteNullValue();
        }
    }

    internal sealed class NewtonsoftStringEnumConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
            if (!enumType.IsEnum) return false;
            var converter = enumType.GetCustomAttribute<Newtonsoft.Json.JsonConverterAttribute>();
            return converter?.ConverterType?.Name == "StringEnumConverter";
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            var enumType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
            if (Nullable.GetUnderlyingType(typeToConvert) is not null)
            {
                return (JsonConverter) Activator.CreateInstance(typeof(NullableNewtonsoftStringEnumConverter<>).MakeGenericType(enumType))!;
            }

            return (JsonConverter) Activator.CreateInstance(typeof(NewtonsoftStringEnumConverter<>).MakeGenericType(enumType))!;
        }

        private sealed class NewtonsoftStringEnumConverter<T> : JsonConverter<T> where T : struct, Enum
        {
            private static readonly IReadOnlyDictionary<string, T> ReadMap = CreateReadMap();
            private static readonly IReadOnlyDictionary<T, string> WriteMap = CreateWriteMap();

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var value = reader.GetString()!;
                    if (ReadMap.TryGetValue(value, out var mapped)) return mapped;
                    if (Enum.TryParse<T>(value, true, out var parsed)) return parsed;
                }

                if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var i))
                {
                    return (T) Enum.ToObject(typeof(T), i);
                }

                throw new JsonException($"Unable to deserialize enum value for {typeof(T).Name}.");
            }

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                if (WriteMap.TryGetValue(value, out var mapped))
                {
                    writer.WriteStringValue(mapped);
                    return;
                }

                writer.WriteStringValue(value.ToString());
            }

            private static IReadOnlyDictionary<string, T> CreateReadMap()
            {
                var map = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
                foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var enumValue = (T) field.GetValue(null)!;
                    var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                    map[field.Name] = enumValue;
                    if (!string.IsNullOrWhiteSpace(enumMember?.Value)) map[enumMember.Value!] = enumValue;
                }

                return new ReadOnlyDictionary<string, T>(map);
            }

            private static IReadOnlyDictionary<T, string> CreateWriteMap()
            {
                var map = new Dictionary<T, string>();
                foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var enumValue = (T) field.GetValue(null)!;
                    var enumMember = field.GetCustomAttribute<EnumMemberAttribute>();
                    map[enumValue] = string.IsNullOrWhiteSpace(enumMember?.Value) ? field.Name : enumMember.Value!;
                }

                return new ReadOnlyDictionary<T, string>(map);
            }
        }

        private sealed class NullableNewtonsoftStringEnumConverter<T> : JsonConverter<T?> where T : struct, Enum
        {
            private static readonly NewtonsoftStringEnumConverter<T> Inner = new NewtonsoftStringEnumConverter<T>();

            public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;
                return Inner.Read(ref reader, typeof(T), options);
            }

            public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
            {
                if (!value.HasValue)
                {
                    writer.WriteNullValue();
                    return;
                }

                Inner.Write(writer, value.Value, options);
            }
        }
    }
}
