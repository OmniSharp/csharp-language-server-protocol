using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public class DapReceiver : IReceiver, IOutputFilter
    {
        private bool _initialized;

        public (IEnumerable<Renor> results, bool hasResponse) GetRequests(JsonElement container)
        {
            var result = GetRenor(container).ToArray();
            return ( result, result.Any(z => z.IsResponse) );
        }

        public bool IsValid(JsonElement container) => container.ValueKind == JsonValueKind.Object;

        protected virtual IEnumerable<Renor> GetRenor(JsonElement @object)
        {
            if (@object.ValueKind != JsonValueKind.Object)
            {
                yield return new InvalidRequest(null, "Not an object");
                yield break;
            }

            if (!@object.TryGetProperty("seq", out var id))
            {
                yield return new InvalidRequest(null, "No sequence given");
                yield break;
            }

            if (!@object.TryGetProperty("type", out var type))
            {
                yield return new InvalidRequest(null, "No type given");
                yield break;
            }

            var sequence = GetInt64(id);
            var messageType = type.GetString();
            var traceState = GetStringProperty(@object, "tracestate");
            var traceParent = GetStringProperty(@object, "traceparent");

            if (messageType == "event")
            {
                if (!@object.TryGetProperty("event", out var @event))
                {
                    yield return new InvalidRequest(null, "No event given");
                    yield break;
                }

                yield return new Notification(@event.GetString()!, @object.TryGetProperty("body", out var body) ? body.Clone() : null) {
                    TraceState = traceState,
                    TraceParent = traceParent
                };
                yield break;
            }

            if (messageType == "request")
            {
                if (!@object.TryGetProperty("command", out var command))
                {
                    yield return new InvalidRequest(null, "No command given");
                    yield break;
                }

                var requestName = command.GetString();
                var requestObject = @object.TryGetProperty("arguments", out var body)
                    ? body.Clone()
                    : JsonSerializer.SerializeToElement(new { });
                if (RequestNames.Cancel == requestName && requestObject.ValueKind == JsonValueKind.Object)
                {
                    // DAP is really weird... the cancellation operation mixes request and progress cancellation.
                    // because we already have the assumption of the cancellation token we are going to just split the request up.
                    // This makes it so that the cancel handler implementer must still return a positive response even if the request didn't make it through.
                    if (requestObject.TryGetProperty("requestId", out var requestId))
                    {
                        yield return new Notification(
                            JsonRpcNames.CancelRequest,
                            JsonSerializer.SerializeToElement(new Dictionary<string, JsonElement> { ["id"] = requestId.Clone() })
                        ) {
                            TraceState = traceState,
                            TraceParent = traceParent
                        };
                        var requestNode = JsonNode.Parse(requestObject.GetRawText())!.AsObject();
                        requestNode.Remove("requestId");
                        requestObject = JsonSerializer.SerializeToElement(requestNode);
                    }
                    else
                    {
                        yield return new Request(sequence, RequestNames.Cancel, requestObject) {
                            TraceState = traceState,
                            TraceParent = traceParent
                        };
                        yield break;
                    }
                }

                {
                    yield return new Request(sequence, requestName, requestObject) {
                        TraceState = traceState,
                        TraceParent = traceParent
                    };
                    yield break;
                }
            }

            if (messageType == "response")
            {
                if (!@object.TryGetProperty("request_seq", out var requestSeq))
                {
                    yield return new InvalidRequest(null, "No request_seq given");
                    yield break;
                }

                if (!@object.TryGetProperty("command", out _))
                {
                    yield return new InvalidRequest(null, "No command given");
                    yield break;
                }

                if (!@object.TryGetProperty("success", out var success))
                {
                    yield return new InvalidRequest(null, "No success given");
                    yield break;
                }

                var bodyValue = @object.TryGetProperty("body", out var body) ? body.Clone() : JsonSerializer.SerializeToElement(new { });

                var requestSequence = GetInt64(requestSeq);
                var successValue = success.GetBoolean();

                if (successValue)
                {
                    yield return new ServerResponse(requestSequence, bodyValue);
                    yield break;
                }

                yield return new ServerError(
                    requestSequence,
                    JsonSerializer.Deserialize<ServerErrorResult>(bodyValue, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new ServerErrorResult(-1, "Unknown Error")
                );
                yield break;
            }

            throw new NotSupportedException($"Message type {messageType} is not supported");
        }

        public void Initialized() => _initialized = true;
        public bool ShouldOutput(object value) => _initialized;

        private static long GetInt64(JsonElement value) => value.ValueKind == JsonValueKind.String
            ? long.Parse(value.GetString()!, CultureInfo.InvariantCulture)
            : value.GetInt64();

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
