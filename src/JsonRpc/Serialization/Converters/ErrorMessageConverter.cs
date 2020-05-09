using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ErrorMessageConverter : JsonConverter<ErrorMessage>
    {
        public override void Write(Utf8JsonWriter writer, ErrorMessage value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("code");
            writer.WriteNumberValue(value.Code);
            if (value.Data != null)
            {
                writer.WritePropertyName("data");
                JsonSerializer.Serialize(writer, value.Data, options);
            }

            writer.WritePropertyName("message");
            writer.WriteStringValue(value.Message);
            writer.WriteEndObject();
        }

        public override ErrorMessage
            Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotImplementedException();
    }
}
