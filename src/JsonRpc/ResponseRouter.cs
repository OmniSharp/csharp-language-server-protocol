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

        public void SendNotification<T>(string method, T @params)
        {
            _outputHandler.Send(new Client.Notification() {
                Method = method,
                Params = @params
            });
        }

        public async Task<TResponse> SendRequest<T, TResponse>(string method, T @params)
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

            try
            {
                var result = await tcs.Task;
                return result.ToObject<TResponse>();
            }
            finally
            {
                _requests.TryRemove(nextId, out var _);
            }
        }

        public async Task SendRequest<T>(string method, T @params)
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
    }
}