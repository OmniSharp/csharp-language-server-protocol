using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Receiver : IReceiver, IOutputFilter
    {
        protected bool _initialized { get; private set; }

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

        public void Initialized()
        {
            _initialized = true;
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

            return ( results, results.Any(z => z.IsResponse) );
        }

        protected virtual Renor GetRenor(JToken @object)
        {
            if (!( @object is JObject request ))
            {
                return new InvalidRequest(null, "Not an object");
            }

            var protocol = request["jsonrpc"]?.Value<string>();
            if (protocol != "2.0")
            {
                return new InvalidRequest(null, "Unexpected protocol");
            }

            object? requestId = null;
            bool hasRequestId;
            // ReSharper disable once AssignmentInConditionalExpression
            if (hasRequestId = request.TryGetValue("id", out var id))
            {
                requestId = id switch
                {
                    { Type: JTokenType.String }  => id.Value<string>(),
                    { Type: JTokenType.Integer } => id.Value<long>(),
                    _ => null
                };
            }

            if (hasRequestId && request.TryGetValue("result", out var response))
            {
                return new ServerResponse(requestId!, response);
            }

            if (request.TryGetValue("error", out var errorResponse))
            {
                // TODO: this doesn't seem right.
                return new ServerError(requestId, errorResponse.ToObject<ServerErrorResult>());
            }

            var method = request["method"]?.Value<string>();
            if (string.IsNullOrEmpty(method))
            {
                return new InvalidRequest(requestId, string.Empty, "Method not set");
            }

            var hasParams = request.TryGetValue("params", out var @params);
            if (hasParams && @params?.Type != JTokenType.Array && @params?.Type != JTokenType.Object && @params?.Type != JTokenType.Null)
            {
                return new InvalidRequest(requestId, method, "Invalid params");
            }

            // Special case params such that if we get a null value (from a non spec compliant system)
            // that we don't fall over and throw an error.
            if (@params?.Type == JTokenType.Null)
            {
                @params = new JObject();
            }

            var properties = request.Properties().ToLookup(z => z.Name, StringComparer.OrdinalIgnoreCase);

            var traceStateProperty = properties["tracestate"].FirstOrDefault();
            var traceState = traceStateProperty?.Value.ToString();
            var traceParentProperty = properties["traceparent"].FirstOrDefault();
            var traceParent = traceParentProperty?.Value.ToString();

            // id == request
            // !id == notification
            if (!hasRequestId)
            {
                return new Notification(method!, @params)
                {
                    TraceState = traceState,
                    TraceParent = traceParent,
                };
            }

            return new Request(requestId!, method!, @params)
            {
                TraceState = traceState,
                TraceParent = traceParent,
            };
        }

        public bool ShouldOutput(object value)
        {
            return _initialized;
        }
    }
}
