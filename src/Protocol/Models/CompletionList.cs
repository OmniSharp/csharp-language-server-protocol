using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    /// <summary>
    /// Represents a collection of [completion items](#CompletionItem) to be presented
    /// in the editor.
    /// </summary>
    [JsonConverter(typeof(Converter))]
    public class CompletionList : Container<CompletionItem>
    {
        public CompletionList() : base(Enumerable.Empty<CompletionItem>())
        {
        }

        public CompletionList(bool isIncomplete) : base(Enumerable.Empty<CompletionItem>())
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(IEnumerable<CompletionItem> items) : base(items)
        {
        }

        public CompletionList(IEnumerable<CompletionItem> items, bool isIncomplete) : base(items)
        {
            IsIncomplete = isIncomplete;
        }

        public CompletionList(params CompletionItem[] items) : base(items)
        {
        }

        public CompletionList(bool isIncomplete, params CompletionItem[] items) : base(items)
        {
            IsIncomplete = isIncomplete;
        }

        /// <summary>
        /// This list it not complete. Further typing should result in recomputing
        /// this list.
        /// </summary>
        public bool IsIncomplete { get; }

        /// <summary>
        /// The completion items.
        /// </summary>
        public IEnumerable<CompletionItem> Items => this;

        public static implicit operator CompletionList(CompletionItem[] items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionList(Collection<CompletionItem> items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionList(List<CompletionItem> items)
        {
            return new CompletionList(items);
        }

        public static implicit operator CompletionItem[](CompletionList list)
        {
            return list.ToArray();
        }

        class Converter : JsonConverter<CompletionList>
        {
            public override void Write(Utf8JsonWriter writer, CompletionList value, JsonSerializerOptions options)
            {
                if (!value.IsIncomplete)
                {
                    JsonSerializer.Serialize(writer, value.Items.ToArray(), options);
                    return;
                }

                writer.WriteStartObject();
                writer.WritePropertyName("isIncomplete");
                writer.WriteBooleanValue(value.IsIncomplete);

                writer.WritePropertyName("items");
                writer.WriteStartArray();
                foreach (var item in value.Items)
                {
                    JsonSerializer.Serialize(writer, item, options);
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }

            public override CompletionList Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    var array = JsonSerializer.Deserialize<List<CompletionItem>>(ref reader, options);
                    return new CompletionList(array);
                }

                if (reader.TokenType != JsonTokenType.StartObject)
                    throw new JsonException("Expected an object");

                string propertyName = null;
                IEnumerable<CompletionItem> items = null;
                var isIncomplete = false;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        reader.Read();
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        propertyName = reader.GetString();
                        continue;
                    }

                    switch (propertyName)
                    {
                        case nameof(items):
                            items = JsonSerializer.Deserialize<List<CompletionItem>>(ref reader, options);
                            break;
                        case nameof(isIncomplete):
                            isIncomplete = reader.GetBoolean();
                            break;
                        default:
                            throw new JsonException($"Unsupported property found {propertyName}");
                    }
                }

                return new CompletionList(items, isIncomplete);
            }
        }
    }
}
