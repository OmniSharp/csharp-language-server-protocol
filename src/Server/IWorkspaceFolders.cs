using System;
using System.Collections.Generic;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Server
{
    public interface IWorkspaceFolders
    {
        IObservable<WorkspaceFolder> Refresh();
        IObservable<IEnumerable<WorkspaceFolder>> WorkspaceFolders { get; }
    }
}