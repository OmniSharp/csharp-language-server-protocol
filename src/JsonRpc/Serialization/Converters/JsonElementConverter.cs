using System;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    internal sealed class JsonElementConverter : JsonConverter<JsonElement>
    {
        public override void WriteJson(JsonWriter writer, JsonElement value, Newtonsoft.Json.JsonSerializer serializer) =>
            writer.WriteRawValue(value.GetRawText());

        public override JsonElement ReadJson(
            JsonReader reader, Type objectType, JsonElement existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer
        )
        {
            var token = JToken.Load(reader);
            using var document = JsonDocument.Parse(token.ToString(Formatting.None));
            return document.RootElement.Clone();
        }
    }
}