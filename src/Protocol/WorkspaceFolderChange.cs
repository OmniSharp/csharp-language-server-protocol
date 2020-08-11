using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    public readonly struct WorkspaceFolderChange
    {
        public WorkspaceFolderChange(WorkspaceFolderEvent @event, WorkspaceFolder folder)
        {
            Event = @event;
            Folder = folder;
        }

        public WorkspaceFolderEvent Event { get; }
        public WorkspaceFolder Folder { get; }
    }
}
