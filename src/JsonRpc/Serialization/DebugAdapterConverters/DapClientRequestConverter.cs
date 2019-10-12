using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters
{
    class DapClientRequestConverter : JsonConverter<Request>
    {
        public override bool CanRead => false;
        public override Request ReadJson(JsonReader reader, Type objectType, Request existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Request value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            writer.WriteValue(value.Id);
            writer.WritePropertyName("type");
            writer.WriteValue("request");
            writer.WritePropertyName("command");
            writer.WriteValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("arguments");
                serializer.Serialize(writer, value.Params);
            }
            writer.WriteEndObject();
        }
    }
}
