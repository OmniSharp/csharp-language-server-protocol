using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.DebugAdapter.Protocol
{
    public class DapReceiver : IReceiver, IOutputFilter
    {
        private bool _initialized;

        public (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            var result = GetRenor(container).ToArray();
            return ( result, result.Any(z => z.IsResponse) );
        }

        public bool IsValid(JToken container)
        {
            if (container is JObject)
            {
                return true;
            }

            return false;
        }

        protected virtual IEnumerable<Renor> GetRenor(JToken @object)
        {
            if (!( @object is JObject request ))
            {
                yield return new InvalidRequest(null, "Not an object");
                yield break;
            }

            if (!request.TryGetValue("seq", out var id))
            {
                yield return new InvalidRequest(null, "No sequence given");
                yield break;
            }

            if (!request.TryGetValue("type", out var type))
            {
                yield return new InvalidRequest(null, "No type given");
                yield break;
            }

            var sequence = id.Value<long>();
            var messageType = type.Value<string>();

            var properties = request.Properties().ToLookup(z => z.Name, StringComparer.OrdinalIgnoreCase);

            if (messageType == "event")
            {
                if (!request.TryGetValue("event", out var @event))
                {
                    yield return new InvalidRequest(null, "No event given");
                    yield break;
                }

                yield return new Notification(@event.Value<string>(), request.TryGetValue("body", out var body) ? body : null) {
                    TraceState = properties["tracestate"].FirstOrDefault()?.Value<string>(),
                    TraceParent = properties["traceparent"].FirstOrDefault()?.Value<string>()
                };
                yield break;
            }

            if (messageType == "request")
            {
                if (!request.TryGetValue("command", out var command))
                {
                    yield return new InvalidRequest(null, "No command given");
                    yield break;
                }

                var requestName = command.Value<string>();
                var requestObject = request.TryGetValue("arguments", out var body) ? body : new JObject();
                if (RequestNames.Cancel == requestName && requestObject is JObject ro)
                {
                    // DAP is really weird... the cancellation operation mixes request and progress cancellation.
                    // because we already have the assumption of the cancellation token we are going to just split the request up.
                    // This makes it so that the cancel handler implementer must still return a positive response even if the request didn't make it through.
                    if (ro.TryGetValue("requestId", out var requestId))
                    {
                        yield return new Notification(JsonRpcNames.CancelRequest, JObject.FromObject(new { id = requestId })) {
                            TraceState = properties["tracestate"].FirstOrDefault()?.Value<string>(),
                            TraceParent = properties["traceparent"].FirstOrDefault()?.Value<string>()
                        };
                        ro.Remove("requestId");
                    }
                    else
                    {
                        yield return new Request(sequence, RequestNames.Cancel, ro) {
                            TraceState = properties["tracestate"].FirstOrDefault()?.Value<string>(),
                            TraceParent = properties["traceparent"].FirstOrDefault()?.Value<string>()
                        };
                        yield break;
                    }
                }

                {
                    yield return new Request(sequence, requestName, requestObject) {
                        TraceState = properties["tracestate"].FirstOrDefault()?.Value<string>(),
                        TraceParent = properties["traceparent"].FirstOrDefault()?.Value<string>()
                    };
                    yield break;
                }
            }

            if (messageType == "response")
            {
                if (!request.TryGetValue("request_seq", out var requestSeq))
                {
                    yield return new InvalidRequest(null, "No request_seq given");
                    yield break;
                }

                if (!request.TryGetValue("command", out _))
                {
                    yield return new InvalidRequest(null, "No command given");
                    yield break;
                }

                if (!request.TryGetValue("success", out var success))
                {
                    yield return new InvalidRequest(null, "No success given");
                    yield break;
                }

                var bodyValue = request.TryGetValue("body", out var body) ? body : new JObject();

                var requestSequence = requestSeq.Value<long>();
                var successValue = success.Value<bool>();

                if (successValue)
                {
                    yield return new ServerResponse(requestSequence, bodyValue);
                    yield break;
                }

                yield return new ServerError(requestSequence, bodyValue?.ToObject<ServerErrorResult>() ?? new ServerErrorResult(-1, "Unknown Error"));
                yield break;
            }

            throw new NotSupportedException($"Message type {messageType} is not supported");
        }

        public void Initialized() => _initialized = true;
        public bool ShouldOutput(object value) => _initialized;
    }
}
