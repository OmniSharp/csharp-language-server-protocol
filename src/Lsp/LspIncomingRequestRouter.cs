using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using JsonRpc.Server;
using Lsp.Messages;

namespace Lsp
{
    public class LspIncomingRequestRouter : IncomingRequestRouter
    {
        private readonly ConcurrentDictionary<string, CancellationTokenSource> _requests = new ConcurrentDictionary<string, CancellationTokenSource>();

        public LspIncomingRequestRouter(HandlerResolver resolver, IServiceProvider serviceProvider) : base(resolver, serviceProvider)
        {
        }

        private string GetId(object id)
        {
            if (id is string s)
            {
                return s;
            }

            if (id is long l)
            {
                return l.ToString();
            }

            return id?.ToString();
        }

        protected async override Task<ErrorResponse> RouteRequest(Request request, CancellationToken token)
        {
            var id = GetId(request.Id);
            var cts = new CancellationTokenSource();
            // Tie to token
            token.Register(cts.Cancel);
            _requests.TryAdd(id, cts);

            // TODO: Try / catch for Internal Error
            try
            {
                return await base.RouteRequest(request, cts.Token);
            }
            catch (TaskCanceledException)
            {
                return new RequestCancelled();
            }
            finally
            {
                _requests.TryRemove(id, out var _);
            }
        }

        public void CancelRequest(object id)
        {
            if (_requests.TryGetValue(GetId(id), out var cts))
            {
                cts.Cancel();
            }
        }
    }
}