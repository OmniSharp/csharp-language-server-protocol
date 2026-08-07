using System;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    internal sealed class JsonElementConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) =>
            objectType == typeof(JsonElement) || objectType == typeof(JsonElement?);

        public override void WriteJson(JsonWriter writer, object? value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is JsonElement element)
            {
                writer.WriteRawValue(element.GetRawText());
                return;
            }

            writer.WriteNull();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            if (token.Type == JTokenType.Null && objectType == typeof(JsonElement?)) return null;

            using var document = JsonDocument.Parse(token.ToString(Formatting.None));
            return document.RootElement.Clone();
        }
    }
}