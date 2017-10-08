using System.Threading.Tasks;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Models;

namespace OmniSharp.Extensions.LanguageServer.Handlers
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
