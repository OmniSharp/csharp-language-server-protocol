﻿//HintName: WorkspaceSymbolRegistrationOptions.cs
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

#nullable enable
namespace Test
{
    [RegistrationOptionsKey(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
    [RegistrationOptionsConverterAttribute(typeof(WorkspaceSymbolRegistrationOptionsConverter))]
    public partial class WorkspaceSymbolRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        [Optional]
        public bool WorkDoneProgress { get; set; }

        class WorkspaceSymbolRegistrationOptionsConverter : RegistrationOptionsConverterBase<WorkspaceSymbolRegistrationOptions, StaticOptions>
        {
            public WorkspaceSymbolRegistrationOptionsConverter()
            {
            }

            public override StaticOptions Convert(WorkspaceSymbolRegistrationOptions source)
            {
                return new StaticOptions
                {
                    WorkDoneProgress = source.WorkDoneProgress
                };
            }
        }

        [RegistrationOptionsKey(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            [Optional]
            public bool WorkDoneProgress { get; set; }
        }
    }
}
#nullable restore
