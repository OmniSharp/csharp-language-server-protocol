using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
    class WorkspaceFoldersManager : IWorkspaceFoldersManager, IDisposable
    {
        private readonly IWorkspaceLanguageClient _client;
        private readonly ConcurrentDictionary<DocumentUri, WorkspaceFolder> _workspaceFolders;
        private readonly ReplaySubject<IEnumerable<WorkspaceFolder>> _workspaceFoldersSubject;

        public WorkspaceFoldersManager(IWorkspaceLanguageClient client)
        {
            _client = client;
            _workspaceFolders = new ConcurrentDictionary<DocumentUri, WorkspaceFolder>(DocumentUri.Comparer);
            _workspaceFoldersSubject = new ReplaySubject<IEnumerable<WorkspaceFolder>>(1);
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

        public void Add(IEnumerable<WorkspaceFolder> workspaceFolders)
        {
            if (!workspaceFolders.Any()) return;

            foreach (var item in workspaceFolders)
            {
                _workspaceFolders.AddOrUpdate(item.Uri, _ => item, (a, b) => item);
            }

            _client.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                Event = new WorkspaceFoldersChangeEvent() {
                    Added = new Container<WorkspaceFolder>(workspaceFolders)
                }
            });
            _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
        }

        public void Add(params WorkspaceFolder[] workspaceFolders)
        {
            if (!workspaceFolders.Any()) return;

            foreach (var item in workspaceFolders)
            {
                _workspaceFolders.AddOrUpdate(item.Uri, _ => item, (a, b) => item);
            }

            _client.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                Event = new WorkspaceFoldersChangeEvent() {
                    Added = new Container<WorkspaceFolder>(workspaceFolders)
                }
            });
            _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
        }

        public void Remove(DocumentUri uri)
        {
            if (_workspaceFolders.TryRemove(uri, out var item))
            {
                _client.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                    Event = new WorkspaceFoldersChangeEvent() {
                        Removed = new Container<WorkspaceFolder>(item)
                    }
                });
                _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
            }
        }

        public void Remove(string name)
        {
            var items = _workspaceFolders.Values.Where(z => z.Name == name).ToArray();
            if (items.Length > 0)
            {
                foreach (var item in items)
                {
                    _workspaceFolders.TryRemove(item.Uri, out _);
                }
                _client.DidChangeWorkspaceFolders(new DidChangeWorkspaceFoldersParams() {
                    Event = new WorkspaceFoldersChangeEvent() {
                        Removed = new Container<WorkspaceFolder>(items)
                    }
                });
                _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
            }
        }

        public void Remove(WorkspaceFolder workspaceFolder)
        {
            Remove(workspaceFolder.Uri);
        }

        public IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders => _workspaceFoldersSubject;

        public IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders => _workspaceFolders.Values;

        public void Dispose()
        {
        }
    }
}
