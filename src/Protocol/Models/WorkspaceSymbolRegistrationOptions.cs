using System.Collections.Generic;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    public class WorkspaceSymbolRegistrationOptions : WorkDoneProgressOptions, IRegistrationOptions
    {
        public class StaticOptions : WorkDoneProgressOptions { }

        class WorkspaceSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<WorkspaceSymbolRegistrationOptions, StaticOptions>
        {
            public WorkspaceSymbolRegistrationOptionsConverter() : base(nameof(ServerCapabilities.WorkspaceSymbolProvider))
            {
            }
            public override StaticOptions Convert(WorkspaceSymbolRegistrationOptions source) => new StaticOptions { WorkDoneProgress = source.WorkDoneProgress };
        }
    }
}
