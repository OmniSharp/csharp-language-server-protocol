using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class RpcErrorConverter : JsonConverter<RpcError>
    {
        public override void Write(Utf8JsonWriter writer, RpcError value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("jsonrpc");
            writer.WriteStringValue("2.0");
            if (value.Id != null)
            {
                writer.WritePropertyName("id");
                JsonSerializer.Serialize(writer, value.Id, options);
            }

            writer.WritePropertyName("error");
            JsonSerializer.Serialize(writer, value.Error, options);
            writer.WriteEndObject();
        }

        public override RpcError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object requestId = null;
            ErrorMessage data = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new NotSupportedException();
            }

            string currentProperty = null;
            while (reader.Read())
            {
                if (currentProperty?.Equals("id", StringComparison.OrdinalIgnoreCase) == true)
                {
                    requestId = reader.TokenType switch {
                        JsonTokenType.String => reader.GetString(),
                        JsonTokenType.Number => reader.GetInt64(),
                        _ => null
                    };
                    currentProperty = null;
                    continue;
                }

                if (currentProperty?.Equals("error", StringComparison.OrdinalIgnoreCase) == true)
                {
                    currentProperty = null;
                    data = JsonSerializer.Deserialize<ErrorMessage>(ref reader, options);
                    continue;
                }

                currentProperty = reader.TokenType switch {
                    JsonTokenType.PropertyName => reader.GetString(),
                    _ => null
                };
            }

            return new RpcError(requestId, data);
        }
    }
}
