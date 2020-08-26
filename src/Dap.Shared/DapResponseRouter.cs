using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.DebugAdapter.Protocol.Requests;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Client;

namespace OmniSharp.Extensions.DebugAdapter.Shared
{
    public class DapResponseRouter : IResponseRouter
    {
        internal readonly IOutputHandler OutputHandler;
        internal readonly ISerializer Serializer;

        internal readonly ConcurrentDictionary<long, (string method, TaskCompletionSource<JToken> pendingTask)> Requests =
            new ConcurrentDictionary<long, (string method, TaskCompletionSource<JToken> pendingTask)>();

        internal static readonly ConcurrentDictionary<Type, string> MethodCache =
            new ConcurrentDictionary<Type, string>();

        public DapResponseRouter(IOutputHandler outputHandler, ISerializer serializer)
        {
            OutputHandler = outputHandler;
            Serializer = serializer;
        }

        public void SendNotification(string method) =>
            OutputHandler.Send(
                new OutgoingNotification {
                    Method = method
                }
            );

        public void SendNotification<T>(string method, T @params) =>
            OutputHandler.Send(
                new OutgoingNotification {
                    Method = method,
                    Params = @params
                }
            );

        public void SendNotification(IRequest @params) => SendNotification(GetMethodName(@params.GetType()), @params);

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken) =>
            SendRequest(GetMethodName(@params.GetType()), @params).Returning<TResponse>(cancellationToken);

        public IResponseRouterReturns SendRequest(string method) => new ResponseRouterReturnsImpl(this, method, new object());

        public IResponseRouterReturns SendRequest<T>(string method, T @params) => new ResponseRouterReturnsImpl(this, method, @params);

        public bool TryGetRequest(long id, [NotNullWhen(true)] out string method, [NotNullWhen(true)] out TaskCompletionSource<JToken> pendingTask)
        {
            var result = Requests.TryGetValue(id, out var source);
            method = source.method;
            pendingTask = source.pendingTask;
            return result;
        }

        private string GetMethodName(Type type)
        {
            if (!MethodCache.TryGetValue(type, out var methodName))
            {
                var attribute = MethodAttribute.From(type);
                if (attribute == null)
                {
                    throw new NotSupportedException($"Unable to infer method name for type {type.FullName}");
                }

                methodName = attribute.Method;
                MethodCache.TryAdd(type, methodName);
            }

            return methodName;
        }

        private class ResponseRouterReturnsImpl : IResponseRouterReturns
        {
            private readonly DapResponseRouter _router;
            private readonly string _method;
            private readonly object _params;

            public ResponseRouterReturnsImpl(DapResponseRouter router, string method, object @params)
            {
                _router = router;
                _method = method;
                _params = @params;
            }

            public async Task<TResponse> Returning<TResponse>(CancellationToken cancellationToken)
            {
                var nextId = _router.Serializer.GetNextId();
                var tcs = new TaskCompletionSource<JToken>();
                _router.Requests.TryAdd(nextId, ( _method, tcs ));

                cancellationToken.ThrowIfCancellationRequested();

                _router.OutputHandler.Send(
                    new OutgoingRequest {
                        Method = _method,
                        Params = _params,
                        Id = nextId
                    }
                );
                if (_method != RequestNames.Cancel)
                {
                    cancellationToken.Register(
                        () => {
                            if (tcs.Task.IsCompleted) return;
                            _router.SendRequest(RequestNames.Cancel, new { requestId = nextId }).Returning<CancelArguments>(CancellationToken.None);
                        }
                    );
                }

                try
                {
                    var result = await tcs.Task;
                    if (typeof(TResponse) == typeof(Unit))
                    {
                        return (TResponse) (object) Unit.Value;
                    }

                    return result.ToObject<TResponse>(_router.Serializer.JsonSerializer);
                }
                finally
                {
                    _router.Requests.TryRemove(nextId, out _);
                }
            }

            public async Task ReturningVoid(CancellationToken cancellationToken) => await Returning<Unit>(cancellationToken);
        }
    }
}
