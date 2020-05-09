using System;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public override Notification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Notification value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            JsonSerializer.Serialize(writer, _serializer.GetNextId(), options);
            writer.WritePropertyName("type");
            writer.WriteStringValue("event");
            writer.WritePropertyName("event");
            writer.WriteStringValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("body");
                JsonSerializer.Serialize(writer, value.Params, options);
            }

            writer.WriteEndObject();
        }
    }
}
