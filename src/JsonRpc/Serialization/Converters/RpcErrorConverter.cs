using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc.Serialization.Converters
{
    public class RpcErrorConverter : JsonConverter<RpcError>
    {
        public override void WriteJson(JsonWriter writer, RpcError value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("jsonrpc");
            writer.WriteValue("2.0");
            if (value.Id != null)
            {
                writer.WritePropertyName("id");
                writer.WriteValue(value.Id);
            }

            writer.WritePropertyName("error");
            serializer.Serialize(writer, value.Error);
            writer.WriteEndObject();
        }

        public override RpcError ReadJson(
            JsonReader reader, Type objectType, RpcError existingValue,
            bool hasExistingValue, JsonSerializer serializer
        )
        {
            var obj = JObject.Load(reader);

            object? requestId = null;
            if (obj.TryGetValue("id", out var id))
            {
                requestId = id switch
                {
                    { Type: JTokenType.String }  => id.Value<string>(),
                    { Type: JTokenType.Integer } => id.Value<long>(),
                    _                            => null
                };
            }

            ErrorMessage? data = null;
            if (obj.TryGetValue("error", out var dataToken))
            {
                data = dataToken.ToObject<ErrorMessage>(serializer);
            }

            return new RpcError(requestId, data);
        }
    }
}
