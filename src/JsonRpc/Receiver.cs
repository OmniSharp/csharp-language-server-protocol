using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class Receiver : IReceiver, IOutputFilter
    {
        protected bool _initialized { get; private set; }

        public bool IsValid(JsonElement container)
        {
            if (container.ValueKind == JsonValueKind.Object)
            {
                return true;
            }

            if (container.ValueKind == JsonValueKind.Array)
            {
                return container.GetArrayLength() > 0;
            }

            return false;
        }

        public void Initialized()
        {
            _initialized = true;
        }

        public virtual (IEnumerable<Renor> results, bool hasResponse) GetRequests(JsonElement container)
        {
            var results = new List<Renor>();

            if (container.ValueKind == JsonValueKind.Array)
            {
                results.AddRange(container.EnumerateArray().Select(GetRenor));
            }
            else
            {
                results.Add(GetRenor(container));
            }

            return ( results, results.Any(z => z.IsResponse) );
        }

        protected virtual Renor GetRenor(JsonElement @object)
        {
            if (@object.ValueKind != JsonValueKind.Object)
            {
                return new InvalidRequest(null, "Not an object");
            }

            var protocol = @object.TryGetProperty("jsonrpc", out var protocolValue) && protocolValue.ValueKind == JsonValueKind.String
                ? protocolValue.GetString()
                : null;
            if (protocol != "2.0")
            {
                return new InvalidRequest(null, "Unexpected protocol");
            }

            object? requestId = null;
            bool hasRequestId;
            // ReSharper disable once AssignmentInConditionalExpression
            if (hasRequestId = @object.TryGetProperty("id", out var id))
            {
                requestId = id.ValueKind switch
                {
                    JsonValueKind.String => id.GetString(),
                    JsonValueKind.Number when id.TryGetInt64(out var numericId) => numericId,
                    _ => null,
                };
            }

            if (hasRequestId && @object.TryGetProperty("result", out var response))
            {
                return new ServerResponse(requestId!, response.Clone());
            }

            if (@object.TryGetProperty("error", out var errorResponse))
            {
                return new ServerError(
                    requestId,
                    JsonSerializer.Deserialize<ServerErrorResult>(errorResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                );
            }

            var method = @object.TryGetProperty("method", out var methodValue)
                ? methodValue.ValueKind == JsonValueKind.String ? methodValue.GetString() : methodValue.ToString()
                : null;
            if (string.IsNullOrEmpty(method))
            {
                return new InvalidRequest(requestId, string.Empty, "Method not set");
            }

            var hasParams = @object.TryGetProperty("params", out var @params);
            if (hasParams && @params.ValueKind is not (JsonValueKind.Array or JsonValueKind.Object or JsonValueKind.Null))
            {
                return new InvalidRequest(requestId, method, "Invalid params");
            }

            // Special case params such that if we get a null value (from a non spec compliant system)
            // that we don't fall over and throw an error.
            if (hasParams && @params.ValueKind == JsonValueKind.Null)
            {
                @params = JsonSerializer.SerializeToElement(new { });
            }

            var traceState = GetStringProperty(@object, "tracestate");
            var traceParent = GetStringProperty(@object, "traceparent");
            JsonElement? paramsValue = hasParams ? @params.Clone() : null;

            // id == request
            // !id == notification
            if (!hasRequestId)
            {
                return new Notification(method!, paramsValue)
                {
                    TraceState = traceState,
                    TraceParent = traceParent,
                };
            }

            return new Request(requestId!, method!, paramsValue)
            {
                TraceState = traceState,
                TraceParent = traceParent,
            };
        }

        public bool ShouldOutput(object value)
        {
            return _initialized;
        }

        private static string? GetStringProperty(JsonElement value, string name)
        {
            foreach (var property in value.EnumerateObject())
            {
                if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return property.Value.ValueKind == JsonValueKind.String ? property.Value.GetString() : property.Value.ToString();
                }
            }

            return null;
        }
    }
}
