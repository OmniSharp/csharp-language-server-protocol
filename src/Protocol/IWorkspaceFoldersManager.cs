using DynamicData;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public interface IWorkspaceFoldersManager
    {
        void Add(DocumentUri uri, string name);
        void Add(params WorkspaceFolder[] workspaceFolders);
        void Remove(DocumentUri uri);
        void Remove(string name);
        void Remove(WorkspaceFolder workspaceFolder);
        IObservableList<WorkspaceFolder> WorkspaceFolders { get; }
    }
}
