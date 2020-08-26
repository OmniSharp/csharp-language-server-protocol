using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.DebugAdapter.Protocol.DebugAdapterConverters
{
    public class DapRpcErrorConverter : JsonConverter
    {
        private readonly ISerializer _serializer;

        public DapRpcErrorConverter(ISerializer serializer) => _serializer = serializer;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!( value is RpcError error ))
            {
                throw new NotSupportedException($"{typeof(RpcError).FullName} was not found!");
            }

            writer.WriteStartObject();
            writer.WritePropertyName("seq");
            writer.WriteValue(_serializer.GetNextId());
            writer.WritePropertyName("type");
            writer.WriteValue("response");
            if (error.Id != null)
            {
                writer.WritePropertyName("request_seq");
                writer.WriteValue(error.Id);
            }

            writer.WritePropertyName("success");
            writer.WriteValue(false);
            writer.WritePropertyName("command");
            writer.WriteValue(error.Method);
            writer.WritePropertyName("message");
            writer.WriteValue(error.Error?.Message);
            writer.WritePropertyName("body");
            serializer.Serialize(writer, error.Error);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            object? requestId = null;
            if (obj.TryGetValue("id", out var id))
            {
                var idString = id.Type == JTokenType.String ? (string) id : null;
                var idLong = id.Type == JTokenType.Integer ? (long?) id : null;
                requestId = idString ?? ( idLong.HasValue ? (object?) idLong.Value : null );
            }

            ErrorMessage? data = null;
            if (obj.TryGetValue("message", out var dataToken))
            {
                data = dataToken.ToObject<ErrorMessage>(serializer);
            }

            return new RpcError(requestId, data);
        }

        public override bool CanConvert(Type objectType) => typeof(RpcError).IsAssignableFrom(objectType);
    }
}
