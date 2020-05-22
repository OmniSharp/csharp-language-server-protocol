using System;
using Newtonsoft.Json;
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
            writer.WritePropertyName("result");
            if (value.Result != null)
            {
                serializer.Serialize(writer, value.Result);
            }
            else{
                writer.WriteNull();
            }
            writer.WriteEndObject();
        }
    }
}
