using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class RpcErrorConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var messageDataType = objectType == typeof(RpcError)
                ? typeof(object)
                : objectType.GetTypeInfo().GetGenericArguments()[0];

            object requestId = null;
            if (obj.TryGetValue("id", out var id))
            {
                var idString = id.Type == JTokenType.String ? (string)id : null;
                var idLong = id.Type == JTokenType.Integer ? (long?)id : null;
                requestId = idString ?? (idLong.HasValue ? (object)idLong.Value : null);
            }

            object data = null;
            if (obj.TryGetValue("error", out var dataToken))
            {
                var errorMessageType = typeof(ErrorMessage<>).MakeGenericType(messageDataType);
                data = dataToken.ToObject(errorMessageType);
            }

            return Activator.CreateInstance(objectType, requestId, data, obj["protocolVersion"].ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RpcError) ||
                   (objectType.GetTypeInfo().IsGenericType && objectType.GetTypeInfo().GetGenericTypeDefinition() == typeof(RpcError<>));
        }

        public override bool CanWrite { get; } = false;
        public override bool CanRead { get; } = true;
    }
}