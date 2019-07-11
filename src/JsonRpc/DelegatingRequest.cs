using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequest<T> : IRequest<JToken>, IRequest
    {
        public DelegatingRequest(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
