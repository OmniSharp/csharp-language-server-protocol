using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceSymbolOptions : WorkDoneProgressOptions, IWorkspaceSymbolOptions
    {
        public static WorkspaceSymbolOptions Of(IWorkspaceSymbolOptions options, IEnumerable<IHandlerDescriptor> descriptors)
        {
            return new WorkspaceSymbolOptions()
            {
                WorkDoneProgress = options.WorkDoneProgress,
            };
        }
    }
}
