using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters
{
    class DapClientNotificationConverter : JsonConverter<Notification>
    {
        private readonly ISerializer _serializer;

        public DapClientNotificationConverter(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public override bool CanRead => false;

        public override Notification ReadJson(JsonReader reader, Type objectType, Notification existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, Notification value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            writer.WriteValue(_serializer.GetNextId());
            writer.WritePropertyName("type");
            writer.WriteValue("event");
            writer.WritePropertyName("event");
            writer.WriteValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("body");
                serializer.Serialize(writer, value.Params);
            }
            writer.WriteEndObject();
        }
    }
}
