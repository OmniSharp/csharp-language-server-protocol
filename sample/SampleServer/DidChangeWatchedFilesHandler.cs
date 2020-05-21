using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Server;

namespace SampleServer
{
    class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private DidChangeWatchedFilesCapability _capability;

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions()
        {
            return new DidChangeWatchedFilesRegistrationOptions();
        }

        public Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken)
        {
            return Unit.Task;
        }

        public void SetCapability(DidChangeWatchedFilesCapability capability)
        {
            _capability = capability;
        }
    }
}
