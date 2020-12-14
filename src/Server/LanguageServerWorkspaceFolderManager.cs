using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    [BuiltIn]
    internal class LanguageServerWorkspaceFolderManager : AbstractHandlers.Base<DidChangeWorkspaceFolderRegistrationOptions>, ILanguageServerWorkspaceFolderManager, IDidChangeWorkspaceFoldersHandler, IOnLanguageServerStarted, IDisposable
    {
        private readonly IWorkspaceLanguageServer _server;
        private readonly ConcurrentDictionary<DocumentUri, WorkspaceFolder> _workspaceFolders;
        private readonly ReplaySubject<IEnumerable<WorkspaceFolder>> _workspaceFoldersSubject;
        private readonly Subject<WorkspaceFolderChange> _workspaceFoldersChangedSubject;

        public LanguageServerWorkspaceFolderManager(IWorkspaceLanguageServer server)
        {
            _server = server;
            _workspaceFolders = new ConcurrentDictionary<DocumentUri, WorkspaceFolder>(DocumentUri.Comparer);
            _workspaceFoldersSubject = new ReplaySubject<IEnumerable<WorkspaceFolder>>(1);
            _workspaceFoldersChangedSubject = new Subject<WorkspaceFolderChange>();
        }

        Task<Unit> IRequestHandler<DidChangeWorkspaceFoldersParams, Unit>.Handle(DidChangeWorkspaceFoldersParams request, CancellationToken cancellationToken)
        {
            if (!IsSupported) return Unit.Task;
            foreach (var folder in request.Event.Added)
            {
                _workspaceFolders.AddOrUpdate(folder.Uri, folder, (_, _) => folder);
                if (_workspaceFoldersChangedSubject.IsDisposed) continue;
                _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Add, folder));
            }

            foreach (var folder in request.Event.Removed)
            {
                _workspaceFolders.TryRemove(folder.Uri, out _);
                if (_workspaceFoldersChangedSubject.IsDisposed) continue;
                _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Remove, folder));
            }

            if (_workspaceFoldersSubject.IsDisposed)
            {
                _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
            }

            return Unit.Task;
        }

        Task IOnLanguageServerStarted.OnStarted(ILanguageServer server, CancellationToken cancellationToken)
        {
            if (IsSupported)
            {
                foreach (var folder in server.ClientSettings.WorkspaceFolders ?? Enumerable.Empty<WorkspaceFolder>())
                {
                    _workspaceFolders.AddOrUpdate(folder.Uri, folder, (_, _) => folder);
                    _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Add, folder));
                }
            }
            return Task.CompletedTask;
        }

        public IObservable<WorkspaceFolder> Refresh() => Observable.Create<WorkspaceFolder>(
            observer => {
                if (!IsSupported) return Observable.Empty<WorkspaceFolder>().Subscribe(observer);
                return Observable.FromAsync(ct => _server.RequestWorkspaceFolders(new WorkspaceFolderParams(), ct))
                                 .Do(
                                      workspaceFolders => {
                                          workspaceFolders ??= new Container<WorkspaceFolder>();
                                          var existingFolders = new HashSet<WorkspaceFolder>(_workspaceFolders.Values.Join(workspaceFolders, z => z.Uri, z => z.Uri, (_, b) => b));
                                          var additions = new HashSet<WorkspaceFolder>();
                                          var removals = new HashSet<WorkspaceFolder>();
                                          foreach (var newFolder in workspaceFolders.Except(existingFolders).ToArray())
                                          {
                                              additions.Add(newFolder);
                                              _workspaceFolders.TryAdd(newFolder.Uri, newFolder);
                                              _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Add, newFolder));
                                          }

                                          foreach (var oldFolder in _workspaceFolders.Values.Except(workspaceFolders).ToArray())
                                          {
                                              removals.Add(oldFolder);
                                              _workspaceFolders.TryAdd(oldFolder.Uri, oldFolder);
                                              _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Remove, oldFolder));
                                          }

                                          if (_workspaceFoldersSubject.IsDisposed) return;
                                          _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
                                      }
                                  )
                                 .SelectMany(_ => _workspaceFolders.Values)
                                 .Subscribe(observer);
            }
        );

        public IObservable<WorkspaceFolderChange> Changed => _workspaceFoldersChangedSubject.AsObservable();
        public IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders => _workspaceFoldersSubject.IsDisposed ? Observable.Empty<IEnumerable<WorkspaceFolder>>() : _workspaceFoldersSubject.AsObservable();
        public IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders => _workspaceFolders.Values;

        public bool IsSupported => ClientCapabilities.Workspace?.WorkspaceFolders.IsSupported == true;

        public void Dispose()
        {
            if (!_workspaceFoldersSubject.IsDisposed) _workspaceFoldersSubject.Dispose();
            if (!_workspaceFoldersChangedSubject.IsDisposed) _workspaceFoldersChangedSubject.Dispose();
        }

        protected override DidChangeWorkspaceFolderRegistrationOptions CreateRegistrationOptions(ClientCapabilities clientCapabilities) => new() {
            Supported = clientCapabilities.Workspace?.WorkspaceFolders == true,
            ChangeNotifications = clientCapabilities.Workspace?.WorkspaceFolders == true
        };
    }
}
