using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface ILanguageClientWorkspaceFoldersManager: IWorkspaceFoldersHandler
    {
        void Add(DocumentUri uri, string name);
        void Add(WorkspaceFolder workspaceFolder, params WorkspaceFolder[] workspaceFolders);
        void Add(IEnumerable<WorkspaceFolder> workspaceFolders);
        void Remove(DocumentUri name);
        void Remove(string name);
        void Remove(WorkspaceFolder workspaceFolder, params WorkspaceFolder[] workspaceFolders);
        void Remove(IEnumerable<WorkspaceFolder> workspaceFolders);
        IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders { get; }
        IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders { get; }
    }
}
