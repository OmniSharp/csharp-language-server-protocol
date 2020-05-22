using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Receiver : IReceiver
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

        public virtual (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            var results = new List<Renor>();

            if (container is JArray)
            {
                results.AddRange(container.Select(GetRenor));
            }
            else
            {
                results.Add(GetRenor(container));
            }
            return (results, results.Any(z => z.IsResponse));
        }

        protected virtual Renor GetRenor(JToken @object)
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

            if (hasRequestId && request.TryGetValue("result", out var response))
            {
                return new ServerResponse(requestId, response);
            }

            if (request.TryGetValue("error", out var errorResponse))
            {
                // TODO: this doesn't seem right.
                return new ServerError(requestId, errorResponse.ToObject<ServerErrorResult>());
            }

            var method = request["method"]?.Value<string>();
            if (string.IsNullOrWhiteSpace(method))
            {
                return new InvalidRequest(requestId, "Method not set");
            }

            var hasParams = request.TryGetValue("params", out var @params);
            if (hasParams && @params?.Type != JTokenType.Array && @params?.Type != JTokenType.Object && @params?.Type != JTokenType.Null)
            {
                return new InvalidRequest(requestId, "Invalid params");
            }

            // Special case params such that if we get a null value (from a non spec compliant system)
            // that we don't fall over and throw an error.
            if (@params?.Type == JTokenType.Null)
            {
                @params = new JObject();
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
