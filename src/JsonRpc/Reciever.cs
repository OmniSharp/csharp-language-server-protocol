using System.Collections.Generic;
using JsonRpc.Server;
using JsonRpc.Server.Messages;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public class Reciever
    {
        public bool IsValid(JToken container)
        {
            // request must be an object or array
            if (container is JObject)
            {
                return true;
            }

            if (container is JArray array)
            {
                return array.Count > 0;
            }

            return false;
        }

        public IEnumerable<ErrorNotificationRequest> GetRequests(JToken container)
        {
            if (container is JArray)
            {
                foreach (var item in container)
                {
                    yield return GetErrorNotificationRequest(item);
                }
                yield break;
            }

            yield return GetErrorNotificationRequest(container);
        }

        private ErrorNotificationRequest GetErrorNotificationRequest(JToken @object)
        {
            if (!(@object is JObject request))
            {
                return new InvalidRequest(null, "Not an object");
            }

            var protocol = request["jsonrpc"]?.Value<string>();
            if (protocol != "2.0")
            {
                return new InvalidRequest(null, "Unexpected protocol");
            }

            object requestId = null;
            bool hasRequestId;
            if (hasRequestId = request.TryGetValue("id", out var id))
            {
                var idString = id.Type == JTokenType.String ? (string)id : null;
                var idLong = id.Type == JTokenType.Integer ? (long?)id : null;
                requestId = idString ?? (idLong.HasValue ? (object)idLong.Value : null);
            }

            var method = request["method"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(method))
            {
                return new InvalidRequest(requestId, "Method not set");
            }

            var hasParams = request.TryGetValue("params", out var @params);
            if (hasParams && @params.Type != JTokenType.Array && @params.Type != JTokenType.Object)
            {
                return new InvalidRequest(requestId, "Invalid params");
            }

            // id == request
            // !id == notification
            if (!hasRequestId)
            {
                return new Notification(method, @params);
            }
            else
            {
                return new Request(requestId, method, @params);
            }
        }
    }
}