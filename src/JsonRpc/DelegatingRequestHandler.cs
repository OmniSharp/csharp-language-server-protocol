using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequestHandler<T, TResponse> : IJsonRpcRequestHandler<DelegatingRequest<T>, JToken>
    {
        private readonly Func<T, CancellationToken, Task<TResponse>> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, CancellationToken, Task<TResponse>> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<JToken> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            var response = await _handler.Invoke(request.Value.ToObject<T>(_serializer.JsonSerializer), cancellationToken).ConfigureAwait(false);
            return JToken.FromObject(response, _serializer.JsonSerializer);
        }
    }

    public class DelegatingRequestHandler<T> : IJsonRpcRequestHandler<DelegatingRequest<T>, JToken>
    {
        private readonly Func<T, CancellationToken, Task> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, CancellationToken, Task> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<JToken> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(request.Value.ToObject<T>(_serializer.JsonSerializer), cancellationToken).ConfigureAwait(false);
            return JValue.CreateNull();
        }
    }
}
