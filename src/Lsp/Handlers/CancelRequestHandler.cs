using System;
using System.Threading;
using System.Threading.Tasks;
using JsonRpc;
using Lsp.Capabilities.Server;
using Lsp.Models;
using Lsp.Protocol;

namespace Lsp.Handlers
{
    public class CancelRequestHandler : ICancelRequestHandler
    {
        private readonly LspRequestRouter _requestRouter;

        internal CancelRequestHandler(LspRequestRouter requestRouter)
        {
            _requestRouter = requestRouter;
        }

        public Task Handle(CancelParams notification)
        {
            _requestRouter.CancelRequest(notification.Id);
            return Task.CompletedTask;
        }
    }

    public class InitializeHandler : IInitializeHandler
    {
        private TaskCompletionSource<InitializeParams> _tcs;

        public InitializeHandler()
        {
            _tcs = new TaskCompletionSource<InitializeParams>();
            Params = _tcs.Task;
        }

        public Task<InitializeParams> Params { get; }

        public async Task<InitializeResult> Handle(InitializeParams request, CancellationToken token)
        {
            //_tcs.SetResult(request);

            //new ServerCapabilities() {
                
            //}
            throw new NotImplementedException();
        }
    }
}
