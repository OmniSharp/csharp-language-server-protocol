using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public delegate Task InitializeDelegate(ILanguageServer server, InitializeParams request, CancellationToken cancellationToken);

    public interface IWorkspaceFolders
    {
        IObservable<WorkspaceFolder> Refresh();
        IObservableList<WorkspaceFolder> WorkspaceFolders { get; }
    }
}
