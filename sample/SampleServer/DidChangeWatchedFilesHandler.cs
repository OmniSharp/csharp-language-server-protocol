using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace SampleServer
{
    internal class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler
    {
        private DidChangeWatchedFilesCapability _capability;

        public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() => new DidChangeWatchedFilesRegistrationOptions();

        public Task<Unit> Handle(DidChangeWatchedFilesParams request, CancellationToken cancellationToken) => Unit.Task;

        public void SetCapability(DidChangeWatchedFilesCapability capability) => _capability = capability;
    }
}
