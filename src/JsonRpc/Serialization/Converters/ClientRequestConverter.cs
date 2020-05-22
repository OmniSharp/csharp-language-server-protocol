using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ClientRequestConverter : JsonConverter<Request>
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
            writer.WritePropertyName("jsonrpc");
            writer.WriteValue("2.0");
            writer.WritePropertyName("id");
            writer.WriteValue(value.Id);
            writer.WritePropertyName("method");
            writer.WriteValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("params");
                serializer.Serialize(writer, value.Params);
            }
            writer.WriteEndObject();
        }
    }
}
