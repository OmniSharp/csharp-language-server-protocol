namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceSymbolOptions : WorkDoneProgressOptions, IWorkspaceSymbolOptions
    {
        public static WorkspaceSymbolOptions Of(IWorkspaceSymbolOptions options)
        {
            return new WorkspaceSymbolOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}