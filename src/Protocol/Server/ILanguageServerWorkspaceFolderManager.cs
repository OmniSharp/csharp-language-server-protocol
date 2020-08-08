using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Server
{
    public interface ILanguageServerWorkspaceFolderManager
    {
        IObservable<WorkspaceFolder> Refresh();
        IObservable<WorkspaceFolderChange> Changed { get; }
        IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders { get; }
        IEnumerable<WorkspaceFolder> CurrentWorkspaceFolders { get; }
        bool IsSupported { get; }
    }
}
