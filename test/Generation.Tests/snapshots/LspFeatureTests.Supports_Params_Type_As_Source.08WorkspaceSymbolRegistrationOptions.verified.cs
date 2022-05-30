//HintName: WorkspaceSymbolRegistrationOptions.cs
using System.Diagnostics;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using OmniSharp.Extensions.LanguageServer.Protocol;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Models
{
    [RegistrationOptionsKey(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
    public partial class WorkspaceSymbolRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        [Optional]
        public bool WorkDoneProgress { get; set; }

        [RegistrationOptionsKey(nameof(ServerCapabilities.WorkspaceSymbolProvider))]
        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            /// <summary>
            /// The server provides support to resolve additional
            /// information for a workspace symbol.
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            [Optional]
            public bool WorkDoneProgress { get; set; }
        }
    }
}
#nullable restore
