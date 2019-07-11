using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequestHandler<T, TResponse> : IJsonRpcRequestHandler<DelegatingRequest<T>, JToken>
    {
        private readonly Func<T, Task<TResponse>> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, Task<TResponse>> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<JToken> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            var response = await _handler.Invoke(request.Value);
            return JToken.FromObject(response, _serializer.JsonSerializer);
        }
    }

    public class DelegatingRequestHandler<T> : IJsonRpcRequestHandler<DelegatingRequest<T>, JToken>
    {
        private readonly Func<T, Task> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, Task> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<JToken> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(request.Value);
            return JValue.CreateNull();
        }
    }
}
