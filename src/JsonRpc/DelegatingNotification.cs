using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingNotification<T> : IRequest
    {
        public DelegatingNotification(object value) => Value = typeof(T) == typeof(Unit) || value is Unit ? new JObject() : JToken.FromObject(value);

        public JToken Value { get; }
    }
}
