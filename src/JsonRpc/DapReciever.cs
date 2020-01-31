using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc.Server;
using OmniSharp.Extensions.JsonRpc.Server.Messages;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DapReceiver : IReceiver
    {
        public (IEnumerable<Renor> results, bool hasResponse) GetRequests(JToken container)
        {
            var result = GetRenor(container);
            return (new[] { result }, result.IsResponse);
        }

        public bool IsValid(JToken container)
        {
            if (container is JObject)
            {
                return true;
            }

            return false;
        }

        protected virtual Renor GetRenor(JToken @object)
        {
            if (!( @object is JObject request ))
            {
                return new InvalidRequest(null, "Not an object");
            }

            if (!request.TryGetValue("seq", out var id))
            {
                return new InvalidRequest(null, "No sequence given");
            }

            if (!request.TryGetValue("type", out var type))
            {
                return new InvalidRequest(null, "No type given");
            }
            var sequence = id.Value<long>();
            var messageType = type.Value<string>();

            if (messageType == "event")
            {
                if (!request.TryGetValue("event", out var @event))
                {
                    return new InvalidRequest(null, "No event given");
                }
                return new Notification(@event.Value<string>(), request.TryGetValue("body", out var body) ? body : null);
            }
            if (messageType == "request")
            {
                if (!request.TryGetValue("command", out var command))
                {
                    return new InvalidRequest(null, "No command given");
                }
                return new Request(sequence, command.Value<string>(), request.TryGetValue("arguments", out var body) ? body : new JObject());
            }
            if (messageType == "response")
            {
                if (!request.TryGetValue("request_seq", out var request_seq))
                {
                    return new InvalidRequest(null, "No request_seq given");
                }
                if (!request.TryGetValue("command", out var command))
                {
                    return new InvalidRequest(null, "No command given");
                }
                if (!request.TryGetValue("success", out var success))
                {
                    return new InvalidRequest(null, "No success given");
                }

                var bodyValue = request.TryGetValue("body", out var body) ? body : null;

                var requestSequence = request_seq.Value<long>();
                var successValue = success.Value<bool>();

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
