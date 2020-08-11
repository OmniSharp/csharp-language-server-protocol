using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    internal class LanguageServerWorkspaceFolderManager : ILanguageServerWorkspaceFolderManager, IDidChangeWorkspaceFoldersHandler, IOnLanguageServerStarted, IDisposable
    {
        private readonly IWorkspaceLanguageServer _server;
        private readonly ConcurrentDictionary<DocumentUri, WorkspaceFolder> _workspaceFolders;
        private readonly ReplaySubject<IEnumerable<WorkspaceFolder>> _workspaceFoldersSubject;
        private readonly Subject<WorkspaceFolderChange> _workspaceFoldersChangedSubject;
        private readonly object _registrationOptions = new object();

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
            foreach (var folder in request.Event?.Added ?? Enumerable.Empty<WorkspaceFolder>())
            {
                _workspaceFolders.AddOrUpdate(folder.Uri, folder, (a, b) => folder);
                _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Add, folder));
            }

            foreach (var folder in request.Event?.Removed ?? Enumerable.Empty<WorkspaceFolder>())
            {
                _workspaceFolders.TryRemove(folder.Uri, out _);
                _workspaceFoldersChangedSubject.OnNext(new WorkspaceFolderChange(WorkspaceFolderEvent.Remove, folder));
            }

            _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
            return Unit.Task;
        }

        async Task IOnLanguageServerStarted.OnStarted(ILanguageServer server, CancellationToken cancellationToken)
        {
            IsSupported = server.ClientSettings?.Capabilities?.Workspace?.WorkspaceFolders.IsSupported == true;
            if (IsSupported)
            {
                await Refresh().LastOrDefaultAsync().ToTask(cancellationToken);
            }
        }

        public IObservable<WorkspaceFolder> Refresh() => Observable.Create<WorkspaceFolder>(
            observer => {
                if (!IsSupported) return Observable.Empty<WorkspaceFolder>().Subscribe(observer);
                return Observable.FromAsync(ct => _server.RequestWorkspaceFolders(new WorkspaceFolderParams(), ct))
                                 .Do(
                                      workspaceFolders => {
                                          var existingFolders = new HashSet<WorkspaceFolder>(_workspaceFolders.Values.Join(workspaceFolders, z => z.Uri, z => z.Uri, (a, b) => b));
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

                                          _workspaceFoldersSubject.OnNext(_workspaceFolders.Values);
                                      }
                                  )
                                 .SelectMany(z => _workspaceFolders.Values)
                                 .Subscribe(observer);
            }
        );

        public IObservable<WorkspaceFolderChange> Changed => _workspaceFoldersChangedSubject.AsObservable();
        public IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders => _workspaceFoldersSubject.AsObservable();
        public IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders => _workspaceFolders.Values;

        public bool IsSupported { get; private set; }

        public object GetRegistrationOptions() => _registrationOptions;

        public void Dispose()
        {
            _workspaceFoldersSubject?.Dispose();
            _workspaceFoldersChangedSubject?.Dispose();
        }
    }
}
