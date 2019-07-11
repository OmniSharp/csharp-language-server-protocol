using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.Embedded.MediatR;

namespace OmniSharp.Extensions.JsonRpc
{
    public class DelegatingNotificationHandler<T> : IJsonRpcNotificationHandler<DelegatingNotification<T>>
    {
        private readonly Action<T> _handler;
        private readonly ISerializer _serializer;

        public DelegatingNotificationHandler(ISerializer serializer, Action<T> handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public Task<Unit> Handle(DelegatingNotification<T> request, CancellationToken cancellationToken)
        {
            _handler.Invoke(request.Value);
            return Unit.Task;
        }
    }

    public class DelegatingNotificationHandler : IJsonRpcNotificationHandler<DelegatingNotification<object>>
    {
        private readonly Action _handler;
        private readonly ISerializer _serializer;

        public DelegatingNotificationHandler(ISerializer serializer, Action handler)
        {
            _handler = handler;
            _serializer = serializer;
        }

        public Task<Unit> Handle(DelegatingNotification<object> request, CancellationToken cancellationToken)
        {
            _handler.Invoke();
            return Unit.Task;
        }
    }
}
