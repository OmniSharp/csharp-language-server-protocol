using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using DynamicData;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    class WorkspaceFoldersManager : IWorkspaceFoldersHandler, IWorkspaceFoldersManager, IDisposable
    {
        private readonly ISourceCache<WorkspaceFolder, DocumentUri> _workspaceFolders;
        private readonly IDisposable _subscription;

        public WorkspaceFoldersManager(ILanguageClient client)
        {
            _workspaceFolders = new SourceCache<WorkspaceFolder, DocumentUri>(x => x.Uri);
            _subscription = _workspaceFolders
                .Connect()
                .Subscribe(z => {
                    var updates = z.Where(x => x.Reason == ChangeReason.Update)
                        .Where(z => z.Previous.HasValue)
                        .ToArray();
                    var adds = z.Where(z => z.Reason == ChangeReason.Add)
                        .Select(z => z.Current)
                        .Concat(updates.Select(z => z.Current));
                    var removes = z.Where(z => z.Reason == ChangeReason.Remove)
                        .Select(z => z.Current)
                        .Concat(updates.Select(z => z.Previous.Value));
                    client.Workspace.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                        Event = new WorkspaceFoldersChangeEvent() {
                            Added = new Container<WorkspaceFolder>(adds),
                            Removed = new Container<WorkspaceFolder>(removes)
                        }
                    });
                });
        }

        Task<Container<WorkspaceFolder>> IRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>>.
            Handle(WorkspaceFolderParams request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Container<WorkspaceFolder>(_workspaceFolders.Items));
        }

        public void Add(DocumentUri uri, string name)
        {
            Add(new WorkspaceFolder() {Name = name, Uri = uri});
        }

        public void Add(IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            _workspaceFolders.AddOrUpdate(workspaceFolders);
        }

        public void Add(params WorkspaceFolder[] workspaceFolders)
        {
            _workspaceFolders.AddOrUpdate(workspaceFolders);
        }

        public void Remove(DocumentUri uri)
        {
            var folder = _workspaceFolders.Items.Where(x => x.Uri == uri);
            _workspaceFolders.AddOrUpdate(folder);
        }

        public void Remove(string name)
        {
            var folder = _workspaceFolders.Items.Where(x => x.Name == name);
            _workspaceFolders.AddOrUpdate(folder);
        }

        public void Remove(WorkspaceFolder workspaceFolder)
        {
            _workspaceFolders.Remove(workspaceFolder);
        }

        public IObservableList<WorkspaceFolder> WorkspaceFolders => _workspaceFolders.Connect().RemoveKey().AsObservableList();

        public void Dispose()
        {
            _subscription.Dispose();
            _workspaceFolders.Dispose();
        }
    }
}
