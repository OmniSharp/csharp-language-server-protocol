using MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingNotificationHandler<T> : IJsonRpcNotificationHandler<DelegatingNotification<T>>
    {
        private readonly ISerializer _serializer;
        private readonly Func<T, CancellationToken, Task> _handler;

        public DelegatingNotificationHandler(ISerializer serializer, Func<T, CancellationToken, Task> handler)
        {
            _serializer = serializer;
            _handler = handler;
        }

        public async Task<Unit> Handle(DelegatingNotification<T> request, CancellationToken cancellationToken)
        {
            await _handler.Invoke(request.Value.ToObject<T>(_serializer.JsonSerializer), cancellationToken).ConfigureAwait(false);
            return Unit.Value;
        }
    }
}
