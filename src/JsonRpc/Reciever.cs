using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JsonRPC.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonRPC
{
    public class Reciever
    {
        public bool IsValid(JToken container)
        {
            if (!(container is JArray array)) return true;
            return array.Any();
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
            if (@object.Type != JTokenType.Object)
            {
                return new ErrorNotificationRequest(new InvalidRequest(null, "Not an object"));
            }

            var protocol = @object["jsonrpc"]?.Value<string>();
            if (protocol != "2.0")
            {
                return new ErrorNotificationRequest(new InvalidRequest(null, "Unexpected protocol"));
            }

            var method = @object["method"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(method))
            {
                return new ErrorNotificationRequest(new InvalidRequest(null, "Method not set"));
            }

            var request = @object as JObject;
            if (request.TryGetValue("params", out var @params) && @params.Type != JTokenType.Array && @params.Type != JTokenType.Object)
            {
                return new ErrorNotificationRequest(new InvalidRequest("Invalid params"));
            }

            // id == request
            // !id == notification
            if (request.TryGetValue("id", out var id))
            {
                var idString = id.Type == JTokenType.String ? (string)id : null;
                var idLong = id.Type == JTokenType.Integer ? (long?)id : null;
                var requestId = idString ?? (idLong.HasValue ? (object)idLong.Value : null);
                return new ErrorNotificationRequest(new Request(requestId, method, @params as JContainer));
            }
            else
            {
                return new ErrorNotificationRequest(new Notification(method, @params as JContainer));
            }
        }
    }
}