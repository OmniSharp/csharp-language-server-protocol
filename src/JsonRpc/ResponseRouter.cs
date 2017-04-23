using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonRpc
{
    public class ResponseRouter : IResponseRouter
    {
        private readonly IOutputHandler _outputHandler;
        private readonly object _lock = new object();
        private long _id = 0;
        private readonly ConcurrentDictionary<long, TaskCompletionSource<JToken>> _requests = new ConcurrentDictionary<long, TaskCompletionSource<JToken>>();

        public ResponseRouter(IOutputHandler outputHandler)
        {
            _outputHandler = outputHandler;
        }

        public Task SendNotification<T>(string method, T @params)
        {
            long nextId;
            lock (_lock)
            {
                nextId = _id++;
            }

            var tcs = new TaskCompletionSource<JToken>();
            _requests.TryAdd(nextId, tcs);

            _outputHandler.Send(new Client.Notification() {
                Method = method,
                Params = @params
            });

            return tcs.Task.ContinueWith(x => _requests.TryRemove(nextId, out var _));
        }

        public Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
        {
            long nextId;
            lock (_lock)
            {
                nextId = _id++;
            }

            var tcs = new TaskCompletionSource<JToken>();
            _requests.TryAdd(nextId, tcs);

            _outputHandler.Send(new Client.Request() {
                Method = method,
                Params = @params,
                Id = nextId
            });

            return tcs.Task
                .ContinueWith(x => {
                    _requests.TryRemove(nextId, out var _);
                    return x.Result.ToObject<TResponse>();
                });
        }

        public Task SendRequest<T>(string method, T @params)
        {
            long nextId;
            lock (_lock)
            {
                nextId = _id++;
            }

            var tcs = new TaskCompletionSource<JToken>();
            _requests.TryAdd(nextId, tcs);

            _outputHandler.Send(new Client.Request() {
                Method = method,
                Params = @params,
                Id = nextId
            });

            return tcs.Task.ContinueWith(x => _requests.TryRemove(nextId, out var _));
        }

        public TaskCompletionSource<JToken> GetRequest(long id)
        {
            _requests.TryGetValue(id, out var source);
            return source;
        }
    }
}