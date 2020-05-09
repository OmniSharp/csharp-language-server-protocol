using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class ClientResponseConverter : JsonConverter<Response>
    {
        public override Response Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Response value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("jsonrpc");
            writer.WriteStringValue("2.0");
            writer.WritePropertyName("id");
            JsonSerializer.Serialize(writer, value.Id, options);
            writer.WritePropertyName("result");
            if (value.Result != null)
            {
                JsonSerializer.Serialize(writer, value.Result, options);
            }
            else
            {
                writer.WriteNullValue();
            }

            writer.WriteEndObject();
        }
    }
}
