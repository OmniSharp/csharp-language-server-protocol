using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        public override void WriteJson(JsonWriter writer, RpcError value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            writer.WriteValue(_serializer.GetNextId());
            writer.WritePropertyName("type");
            writer.WriteValue("response");
            if (value.Id != null)
            {
                writer.WritePropertyName("request_seq");
                writer.WriteValue(long.Parse((string) value.Id));
            }
            writer.WritePropertyName("success");
            writer.WriteValue(false);
            writer.WritePropertyName("message");
            writer.WriteValue(value.Error.Data);
            writer.WriteEndObject();
        }

        public override RpcError ReadJson(JsonReader reader, Type objectType, RpcError existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {

            var obj = JObject.Load(reader);

            object requestId = null;
            if (obj.TryGetValue("id", out var id))
            {
                var idString = id.Type == JTokenType.String ? (string) id : null;
                var idLong = id.Type == JTokenType.Integer ? (long?) id : null;
                requestId = idString ?? ( idLong.HasValue ? (object) idLong.Value : null );
            }

            ErrorMessage data = null;
            if (obj.TryGetValue("message", out var dataToken))
            {
                var errorMessageType = typeof(ErrorMessage);
                data = dataToken.ToObject<ErrorMessage>(serializer);
            }

            return new RpcError(requestId, data);
        }
    }
}
