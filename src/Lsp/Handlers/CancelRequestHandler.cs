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
}
