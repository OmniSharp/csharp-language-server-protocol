using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingJsonRequestHandler : IJsonRpcRequestHandler<DelegatingRequest<JToken>, JToken>
    {
        private readonly Func<JToken, CancellationToken, Task<JToken>> _handler;

        public DelegatingJsonRequestHandler(Func<JToken, CancellationToken, Task<JToken>> handler) => _handler = handler;

        public async Task<JToken> Handle(DelegatingRequest<JToken> request, CancellationToken cancellationToken)
        {
            var response = await _handler.Invoke(request.Value, cancellationToken).ConfigureAwait(false);
            return response;
        }
    }
}
