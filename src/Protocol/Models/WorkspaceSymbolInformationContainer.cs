using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceSymbolInformationContainer : Container<WorkspaceSymbolInformation>
    {
        public WorkspaceSymbolInformationContainer() : this(Enumerable.Empty<WorkspaceSymbolInformation>())
        {
        }

        public WorkspaceSymbolInformationContainer(IEnumerable<WorkspaceSymbolInformation> items) : base(items)
        {
        }

        public WorkspaceSymbolInformationContainer(params WorkspaceSymbolInformation[] items) : base(items)
        {
        }

        public static implicit operator WorkspaceSymbolInformationContainer(WorkspaceSymbolInformation[] items)
        {
            return new WorkspaceSymbolInformationContainer(items);
        }

        public static implicit operator WorkspaceSymbolInformationContainer(Collection<WorkspaceSymbolInformation> items)
        {
            return new WorkspaceSymbolInformationContainer(items);
        }

        public static implicit operator WorkspaceSymbolInformationContainer(List<WorkspaceSymbolInformation> items)
        {
            return new WorkspaceSymbolInformationContainer(items);
        }
    }
}
