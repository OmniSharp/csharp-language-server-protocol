using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingJsonNotificationHandler : IJsonRpcNotificationHandler<DelegatingNotification<JsonElement>>
    {
        private readonly Func<JsonElement, CancellationToken, Task> _handler;

        public DelegatingJsonNotificationHandler(Func<JsonElement, CancellationToken, Task> handler) => _handler = handler;

        public async Task<Unit> Handle(DelegatingNotification<JsonElement> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(request.Value, cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
