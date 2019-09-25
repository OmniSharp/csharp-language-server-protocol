using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ClientResponseConverter : JsonConverter<Response>
    {
        public override bool CanRead => false;
        public override Response ReadJson(JsonReader reader, Type objectType, Response existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Response value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("jsonrpc");
            writer.WriteValue("2.0");
            writer.WritePropertyName("id");
            writer.WriteValue(value.Id);
            if (value.Result != null)
            {
                writer.WritePropertyName("result");
                serializer.Serialize(writer, value.Result);
            }
            writer.WriteEndObject();
        }
    }
}
