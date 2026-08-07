using System;
using System.Threading;
using System.Threading.Tasks;
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
            var response = await _handler.Invoke(_serializer.DeserializeObject<T>(request.Value), cancellationToken).ConfigureAwait(false);
            return JToken.Parse(_serializer.SerializeObject(response));
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
            await _handler.Invoke(_serializer.DeserializeObject<T>(request.Value), cancellationToken).ConfigureAwait(false);
            return JValue.CreateNull();
        }
    }
}
