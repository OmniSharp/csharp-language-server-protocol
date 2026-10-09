using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingJsonRequestHandler : IJsonRpcRequestHandler<DelegatingRequest<JsonElement>, JsonElement>
    {
        private readonly Func<JsonElement, CancellationToken, Task<JsonElement>> _handler;

        public DelegatingJsonRequestHandler(Func<JsonElement, CancellationToken, Task<JsonElement>> handler) => _handler = handler;

        public async Task<JsonElement> Handle(DelegatingRequest<JsonElement> request, CancellationToken cancellationToken)
        {
            var response = await _handler.Invoke(request.Value, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }
}
