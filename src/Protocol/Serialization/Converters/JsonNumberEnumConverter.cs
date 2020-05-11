using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters
{
    public sealed class JsonNumberEnumConverter : JsonConverterFactory
    {
        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsEnum;

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Activator.CreateInstance(typeof(JsonNumberEnumConverter<>).MakeGenericType(typeToConvert)) as
                JsonConverter;
        }
    }

    class JsonNumberEnumConverter<T> : JsonConverter<T> where T : struct
    {
        public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().IsEnum;

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return (T) Enum.ToObject(Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert, reader.GetInt32());
            }

            return (T) Enum.Parse(Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert, reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(Convert.ToInt32(value));
    }
}
