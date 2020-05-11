using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DapReceiver// : IReceiver
    {
        private readonly JsonDocument _emptyObject;
        private readonly JsonElement _noneObject;

        public DapReceiver()
        {
            _emptyObject = JsonDocument.Parse("{}");
            _noneObject = new JsonElement();
        }

        public (IEnumerable<Renor> results, bool hasResponse) GetRequests(JsonElement container)
        {
            var result = GetRenor(container);
            return (new[] { result }, result.IsResponse);
        }

        public bool IsValid(JsonElement container)
        {
            return container.ValueKind == JsonValueKind.Object;
        }

        protected virtual Renor GetRenor(JsonElement request)
        {
            if (request.ValueKind != JsonValueKind.Object)
            {
                return new InvalidRequest(null, "Not an object");
            }

            if (!request.TryGetProperty("seq", out var id))
            {
                return new InvalidRequest(null, "No sequence given");
            }

            if (!request.TryGetProperty("type", out var type))
            {
                return new InvalidRequest(null, "No type given");
            }

            var sequence = id.GetInt64();
            var messageType = type.GetString();

            if (messageType == "event")
            {
                if (!request.TryGetProperty("event", out var @event))
                {
                    return new InvalidRequest(null, "No event given");
                }
                return new Notification(@event.GetString(), request.TryGetProperty("body", out var body) ? body  : _noneObject);
            }

            if (messageType == "request")
            {
                if (!request.TryGetProperty("command", out var command))
                {
                    return new InvalidRequest(null, "No command given");
                }
                return new Request(sequence, command.GetString(), request.TryGetProperty("arguments", out var body) ? body : _emptyObject.RootElement);
            }

            if (messageType == "response")
            {
                if (!request.TryGetProperty("request_seq", out var request_seq))
                {
                    return new InvalidRequest(null, "No request_seq given");
                }
                if (!request.TryGetProperty("command", out var command))
                {
                    return new InvalidRequest(null, "No command given");
                }
                if (!request.TryGetProperty("success", out var success))
                {
                    return new InvalidRequest(null, "No success given");
                }

                var bodyValue = request.TryGetProperty("body", out var body) ? body : _noneObject;

                var requestSequence = request_seq.GetInt64();
                var successValue = success.GetBoolean();

                if (successValue)
                {
                    return new ServerResponse(requestSequence, bodyValue);
                }
                return new ServerError(requestSequence, bodyValue);
            }

            throw new NotSupportedException($"Message type {messageType} is not supported");
        }
    }
}
