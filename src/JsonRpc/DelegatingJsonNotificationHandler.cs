using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingJsonNotificationHandler : IJsonRpcNotificationHandler<DelegatingNotification<JToken>>
    {
        private readonly Func<JToken, CancellationToken, Task> _handler;

        public DelegatingJsonNotificationHandler(Func<JToken, CancellationToken, Task> handler) => _handler = handler;

        public async Task<Unit> Handle(DelegatingNotification<JToken> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(request.Value, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
