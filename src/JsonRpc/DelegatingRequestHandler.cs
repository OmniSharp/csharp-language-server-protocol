using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingRequestHandler<T, TResponse> : IJsonRpcRequestHandler<DelegatingRequest<T>, Memory<byte>>
    {
        private readonly Func<T, CancellationToken, Task<TResponse>> _handler;
        private readonly ISerializer _serializer;

        public DelegatingRequestHandler(ISerializer serializer, Func<T, CancellationToken, Task<TResponse>> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public async Task<Memory<byte>> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            var response = await _handler.Invoke(request.Value, cancellationToken);
            return new Memory<byte>(JsonSerializer.SerializeToUtf8Bytes(response, _serializer.Options));
        }
    }

    public class DelegatingRequestHandler<T> : IJsonRpcRequestHandler<DelegatingRequest<T>, Memory<byte>>
    {
        private readonly Func<T, CancellationToken, Task> _handler;
        private static readonly byte[] NullMemory = JsonSerializer.SerializeToUtf8Bytes(null, typeof(object));

        public DelegatingRequestHandler(Func<T, CancellationToken, Task> handler)
        {
            _handler = handler;
        }

        public async Task<Memory<byte>> Handle(DelegatingRequest<T> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(request.Value, cancellationToken);
            return new Memory<byte>(NullMemory);
        }
    }
}
