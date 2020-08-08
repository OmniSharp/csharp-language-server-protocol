using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Client
{
    class LanguageClientWorkspaceFoldersManager : ILanguageClientWorkspaceFoldersManager, IDisposable
    {
        private readonly IWorkspaceLanguageClient _client;
        private readonly ConcurrentDictionary<DocumentUri, WorkspaceFolder> _workspaceFolders;
        private readonly ReplaySubject<IEnumerable<WorkspaceFolder>> _workspaceFoldersSubject;

        public LanguageClientWorkspaceFoldersManager(IWorkspaceLanguageClient client, IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            _client = client;
            _workspaceFolders = new ConcurrentDictionary<DocumentUri, WorkspaceFolder>(DocumentUri.Comparer);
            _workspaceFoldersSubject = new ReplaySubject<IEnumerable<WorkspaceFolder>>(1);

            foreach (var folder in workspaceFolders)
            {
                _workspaceFolders.TryAdd(folder.Uri, folder);
            }
        }

        Task<Container<WorkspaceFolder>> IRequestHandler<WorkspaceFolderParams, Container<WorkspaceFolder>>.
            Handle(WorkspaceFolderParams request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new Container<WorkspaceFolder>(_workspaceFolders.Values));
        }

        public void Add(DocumentUri uri, string name)
        {
            Add(new WorkspaceFolder() {Name = name, Uri = uri});
        }

        public void Add(WorkspaceFolder folder, params WorkspaceFolder[] workspaceFolders)
        {
            Add(new[] {folder}.Concat(workspaceFolders));
        }

        public void Add(IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            var additions = new HashSet<WorkspaceFolder>();
            foreach (var item in workspaceFolders.Except(_workspaceFolders.Values).ToArray())
            {
                if (_workspaceFolders.TryAdd(item.Uri, item))
                {
                    additions.Add(item);
                }
            }

            if (additions.Count == 0) return;

            _client.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                Event = new WorkspaceFoldersChangeEvent() {
                    Added = new Container<WorkspaceFolder>(additions)
                }
            });
            _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
        }

        public void Remove(DocumentUri name)
        {
            Remove(_workspaceFolders.Values.Where(z => z.Uri == name));
        }

        public void Remove(string name)
        {
            Remove(_workspaceFolders.Values.Where(z => z.Name == name));
        }

        public void Remove(WorkspaceFolder folder, params WorkspaceFolder[] workspaceFolders)
        {
            Remove(new [] { folder }.Concat(workspaceFolders));
        }

        public void Remove(IEnumerable<WorkspaceFolder> items)
        {
            var removals = new HashSet<WorkspaceFolder>();
            foreach (var item in items.ToArray())
            {
                if (_workspaceFolders.TryRemove(item.Uri, out _))
                {
                    removals.Add(item);
                }
            }

            if (removals.Count == 0) return;

            _client.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                Event = new WorkspaceFoldersChangeEvent() {
                    Removed = new Container<WorkspaceFolder>(removals)
                }
            });
            _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
        }

        public IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders => _workspaceFoldersSubject.AsObservable();

        public IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders => _workspaceFolders.Values;

        public void Dispose()
        {
            _workspaceFoldersSubject?.Dispose();
        }
    }
}
