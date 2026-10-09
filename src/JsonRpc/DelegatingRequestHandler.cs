using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequestHandler<T, TResponse> : IJsonRpcRequestHandler<DelegatingRequest<T>, JsonElement>
    {
        private readonly Func<T, CancellationToken, Task<TResponse>> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, CancellationToken, Task<TResponse>> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<JsonElement> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            var response = await _handler.Invoke(_serializer.DeserializeObject<T>(request.Value), cancellationToken).ConfigureAwait(false);
            using var document = JsonDocument.Parse(_serializer.SerializeObject(response));
            return document.RootElement.Clone();
        }
    }

    public class DelegatingRequestHandler<T> : IJsonRpcRequestHandler<DelegatingRequest<T>, JsonElement>
    {
        private readonly Func<T, CancellationToken, Task> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, CancellationToken, Task> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<JsonElement> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(_serializer.DeserializeObject<T>(request.Value), cancellationToken).ConfigureAwait(false);
            return JsonSerializer.SerializeToElement<object?>(null);
        }
    }
}
