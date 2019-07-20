using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters
{
    class DapClientResponseConverter : JsonConverter<Response>
    {
        private readonly ISerializer _serializer;

        public DapClientResponseConverter(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public override bool CanRead => false;
        public override Response ReadJson(JsonReader reader, Type objectType, Response existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Response value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            writer.WriteValue(_serializer.GetNextId());
            writer.WritePropertyName("type");
            writer.WriteValue("response");
            writer.WritePropertyName("request_seq");
            writer.WriteValue(value.Id);
            writer.WritePropertyName("command");
            writer.WriteValue(value.Request?.Method);
            if (value.Result != null)
            {
                writer.WritePropertyName("body");
                serializer.Serialize(writer, value.Result);
            }
            writer.WriteEndObject();
        }
    }
}
