using System;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.DebugAdapterConverters
{
    internal class DapClientNotificationConverter : JsonConverter<OutgoingNotification>
    {
        private readonly ISerializer _serializer;

        public DapClientNotificationConverter(ISerializer serializer) => _serializer = serializer;

        public override bool CanRead => false;

        public override OutgoingNotification ReadJson(
            JsonReader reader, Type objectType, OutgoingNotification existingValue,
            bool hasExistingValue, JsonSerializer serializer
        ) =>
            throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, OutgoingNotification value, JsonSerializer serializer)
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
