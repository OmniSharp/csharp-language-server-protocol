using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IWorkspaceFoldersManager: IWorkspaceFoldersHandler
    {
        void Add(DocumentUri uri, string name);
        void Add(params WorkspaceFolder[] workspaceFolders);
        void Remove(DocumentUri uri);
        void Remove(string name);
        void Remove(WorkspaceFolder workspaceFolder);
        IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders { get; }
        IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders { get; }
    }
}
