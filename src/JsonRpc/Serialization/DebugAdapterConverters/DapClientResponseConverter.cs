using System;
using System.Text.Json;
using System.Text.Json.Serialization;
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

        public override Response Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Response value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            JsonSerializer.Serialize(writer, _serializer.GetNextId(), options);
            writer.WritePropertyName("type");
            writer.WriteStringValue("response");
            writer.WritePropertyName("request_seq");
            JsonSerializer.Serialize(writer, value.Id, options);
            // TODO: Dynamically set this based on handler execution.
            writer.WritePropertyName("success");
            writer.WriteBooleanValue(true);
            writer.WritePropertyName("command");
            writer.WriteStringValue(value.Request?.Method);
            if (value.Result != null)
            {
                writer.WritePropertyName("body");
                  JsonSerializer.Serialize(writer, value.Result, options);
            }
            writer.WriteEndObject();
        }
    }
}
