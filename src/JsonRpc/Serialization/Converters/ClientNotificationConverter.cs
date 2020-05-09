using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ClientNotificationConverter : JsonConverter<Notification>
    {
        public override Notification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Notification value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("jsonrpc");
            writer.WriteStringValue("2.0");
            writer.WritePropertyName("method");
            writer.WriteStringValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("params");
                JsonSerializer.Serialize(writer, value.Params, options);
            }

            writer.WriteEndObject();
        }
    }
}
