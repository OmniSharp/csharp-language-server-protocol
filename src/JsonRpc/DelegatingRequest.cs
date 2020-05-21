using Newtonsoft.Json.Linq;
using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequest<T> : IRequest<JToken>, IRequest
    {
        public DelegatingRequest(object value)
        {
            Value = typeof(T) == typeof(Unit) || value is Unit ? new JObject() : JToken.FromObject(value);
        }

        public JToken Value { get; }
    }
}
