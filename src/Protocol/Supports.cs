using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    [JsonConverter(typeof(Supports.ConverterFactory))]
    public struct Supports<T> : ISupports
    {
        private readonly bool? _isSupported;

        public Supports(bool? isSupported, T value)
        {
            _isSupported = isSupported;
            Value = value;
        }

        public Supports(bool? isSupported)
        {
            _isSupported = isSupported;
            Value = default(T);
        }

        public T Value { get; set; }
        public bool IsSupported => _isSupported ?? false;
        public Type ValueType => typeof(T);
        object ISupports.Value => Value;

        public static implicit operator T(Supports<T> value)
        {
            return value.Value;
        }

        public static implicit operator Supports<T>(T value)
        {
            return new Supports<T>(true, value);
        }
    }

    public static class Supports
    {
        public static Supports<T> OfValue<T>(T value)
        {
            return new Supports<T>(value != null, value);
        }

        public static Supports<T> OfBoolean<T>(bool? isSupported)
        {
            return new Supports<T>(isSupported);
        }

        internal class ConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type objectType) => typeof(ISupports).IsAssignableFrom(objectType) && objectType.IsGenericType;

            public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
                Activator.CreateInstance(typeof(Converter<>).MakeGenericType(typeToConvert).GetGenericArguments()[0]) as JsonConverter;
        }

        internal class Converter<T> : JsonConverter<Supports<T>>
        {
            public override void Write(Utf8JsonWriter writer, Supports<T> value, JsonSerializerOptions options)
            {
                if (value.IsSupported)
                {
                    if (value.Value != null)
                    {
                        JsonSerializer.Serialize(writer, value.Value);
                    }
                    else
                    {
                        writer.WriteBooleanValue(true);
                    }
                }
                else
                {
                    writer.WriteBooleanValue(false);
                }
            }

            public override Supports<T> Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False)
                {
                    return typeof(T) == typeof(bool)
                        ? Supports.OfValue((T) (object) reader.GetBoolean())
                        : Supports.OfBoolean<T>(reader.GetBoolean());
                }

                if (typeof(T) == typeof(bool))
                {
                    return new Supports<T>(true, (T) (object) true);
                }

                return Supports.OfValue(JsonSerializer.Deserialize<T>(ref reader, options));
            }

            public override bool CanConvert(Type objectType) =>
                objectType.GetTypeInfo().IsGenericType &&
                objectType.GetGenericTypeDefinition() == typeof(Supports<>);
        }

        internal class PropertyConverter<T> : JsonConverter<T> where T: new()
        {
            private readonly IDictionary<string, (string name, PropertyInfo propertyInfo)> _properties = typeof(T)
                .GetProperties()
                .ToDictionary(
                    x => x.Name[0].ToString().ToLower() + x.Name.Substring(1),
                    x => (x.Name[0].ToString().ToLower() + x.Name.Substring(1), x),
                    StringComparer.OrdinalIgnoreCase
                );

            public override T Read(ref Utf8JsonReader reader, Type typeToConvert,
                JsonSerializerOptions options)
            {
                var cap = new T();
                if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected an object");
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = reader.GetString();
                        reader.Read();
                        var (_, property) = _properties[propertyName];
                        property.SetValue(cap, JsonSerializer.Deserialize(ref reader, property.PropertyType, options));
                    }
                }

                reader.Read();
                return cap;
            }

            public override void Write(Utf8JsonWriter writer, T value,
                JsonSerializerOptions options)
            {
                foreach (var (name, property) in _properties.Values)
                {
                    var propertyValue = property.GetValue(value);
                    if (propertyValue is ISupports supports)
                    {
                        if (!supports.IsSupported) continue;
                        writer.WritePropertyName(name);
                        JsonSerializer.Serialize(writer, supports.Value, options);
                    }
                    else
                    {
                        writer.WritePropertyName(name);
                        JsonSerializer.Serialize(writer, propertyValue, options);
                    }
                }
            }
        }
    }
}
