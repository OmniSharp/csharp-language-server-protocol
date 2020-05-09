using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters
{
    class DapClientRequestConverter : JsonConverter<Request>
    {
        public override Request Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Request value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WritePropertyName("type");
            writer.WriteStringValue("request");
            writer.WritePropertyName("command");
            writer.WriteStringValue(value.Method);
            if (value.Params != null)
            {
                writer.WritePropertyName("arguments");
                JsonSerializer.Serialize(writer, value.Params, options);
            }

            writer.WriteEndObject();
        }
    }
}
