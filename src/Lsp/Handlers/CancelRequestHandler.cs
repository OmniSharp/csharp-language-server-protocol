using System.Threading.Tasks;
using Lsp.Protocol;
using OmniSharp.Extensions.LanguageServerProtocol.Models;

namespace OmniSharp.Extensions.LanguageServerProtocol.Handlers
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
