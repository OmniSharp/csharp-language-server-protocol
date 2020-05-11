using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Serialization.DebugAdapterConverters
{
    public class DapRpcErrorConverter : JsonConverter<RpcError>
    {
        private readonly ISerializer _serializer;

        public DapRpcErrorConverter(ISerializer serializer)
        {
            _serializer = serializer;
        }

        public override void Write(Utf8JsonWriter writer, RpcError value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            JsonSerializer.Serialize(writer, _serializer.GetNextId(), options);
            writer.WritePropertyName("type");
            writer.WriteStringValue("response");
            if (value.Id != null)
            {
                writer.WritePropertyName("request_seq");
                JsonSerializer.Serialize(writer, value.Id, options);
            }
            writer.WritePropertyName("success");
            writer.WriteBooleanValue(false);
            writer.WritePropertyName("message");
            JsonSerializer.Serialize(writer, value.Error.Data, options);
            writer.WriteEndObject();
        }

        public override RpcError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object requestId = null;
            ErrorMessage data = null;

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Expected an object");
            }

            string currentProperty = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject) { reader.Read(); break;}
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

                if (currentProperty?.Equals("message", StringComparison.OrdinalIgnoreCase) == true)
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
