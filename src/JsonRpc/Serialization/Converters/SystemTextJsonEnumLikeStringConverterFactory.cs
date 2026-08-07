using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class SystemTextJsonEnumLikeStringConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) => typeof(IEnumLikeString).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
            (JsonConverter) Activator.CreateInstance(typeof(EnumLikeStringConverter<>).MakeGenericType(typeToConvert))!;

        private sealed class EnumLikeStringConverter<T> : JsonConverter<T> where T : struct, IEnumLikeString
        {
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                (T) Activator.CreateInstance(typeof(T), reader.GetString())!;

            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }
    }
}