using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;

namespace OmniSharp.Extensions.JsonRpc
{
    public class ResponseRouter : IResponseRouter
    {
        private readonly IOutputHandler _outputHandler;
        private readonly ISerializer _serializer;
        private readonly object _lock = new object();
        private readonly ConcurrentDictionary<long, TaskCompletionSource<JToken>> _requests = new ConcurrentDictionary<long, TaskCompletionSource<JToken>>();
        private static readonly ConcurrentDictionary<Type, string> _methodCache = new ConcurrentDictionary<Type, string>();

        public ResponseRouter(IOutputHandler outputHandler, ISerializer serializer)
        {
            _outputHandler = outputHandler;
            _serializer = serializer;
        }

        public void SendNotification(string method)
        {
            _outputHandler.Send(new Client.Notification() {
                Method = method
            }, CancellationToken.None);
        }

        public void SendNotification<T>(string method, T @params)
        {
            _outputHandler.Send(new Client.Notification() {
                Method = method,
                Params = @params
            }, CancellationToken.None);
        }

        public void SendNotification(IRequest @params)
        {
            SendNotification(GetMethodName(@params.GetType()), @params);
        }

        public async Task<TResponse> SendRequest<T, TResponse>(string method, T @params, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<JToken>();

            var nextId = _serializer.GetNextId();
            _requests.TryAdd(nextId, tcs);

            _outputHandler.Send(new Client.Request() {
                Method = method,
                Params = @params,
                Id = nextId
            }, cancellationToken);

            try
            {
                var result = await tcs.Task;
                if (typeof(TResponse) == typeof(Unit))
                {
                    return (TResponse)(object)Unit.Value;
                }
                return result.ToObject<TResponse>(_serializer.JsonSerializer);
            }
            finally
            {
                _requests.TryRemove(nextId, out _);
            }
        }

        public Task<TResponse> SendRequest<TResponse>(IRequest<TResponse> @params, CancellationToken cancellationToken)
        {
            return SendRequest<IRequest<TResponse>, TResponse>(GetMethodName(@params.GetType()), @params, cancellationToken);
        }

        public Task SendRequest(IRequest @params, CancellationToken cancellationToken)
        {
            return SendRequest(GetMethodName(@params.GetType()), @params, cancellationToken);
        }

        public async Task<TResponse> SendRequest<TResponse>(string method, CancellationToken cancellationToken)
        {
            var nextId = _serializer.GetNextId();

            var tcs = new TaskCompletionSource<JToken>();
            _requests.TryAdd(nextId, tcs);

            _outputHandler.Send(new Client.Request() {
                Method = method,
                Params = null,
                Id = nextId
            }, cancellationToken);

            try
            {
                var result = await tcs.Task;
                return result.ToObject<TResponse>(_serializer.JsonSerializer);
            }
            finally
            {
                _requests.TryRemove(nextId, out var _);
            }
        }

        public async Task SendRequest<T>(string method, T @params, CancellationToken cancellationToken)
        {
            var nextId = _serializer.GetNextId();

            var tcs = new TaskCompletionSource<JToken>();
            _requests.TryAdd(nextId, tcs);

            _outputHandler.Send(new Client.Request() {
                Method = method,
                Params = @params,
                Id = nextId
            }, cancellationToken);

            try
            {
                await tcs.Task;
            }
            finally
            {
                _requests.TryRemove(nextId, out var _);
            }
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            _requests.TryGetValue(id, out var source);
            return source;
        }

        private string GetMethodName(Type type)
        {
            if (!_methodCache.TryGetValue(type, out var methodName))
            {
                var attribute = type.GetCustomAttribute<MethodAttribute>(true);
                if (attribute == null)
                {
                    throw new NotSupportedException($"Unable to infer method name for type {type.FullName}");
                }

                methodName = attribute.Method;
                _methodCache.TryAdd(type, methodName);
            }

            return methodName;
        }
    }
}
