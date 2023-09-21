//HintName: SubLensRegistrationOptions.cs
using System.Diagnostics;
using System.Linq;
using MediatR;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test.Models
{
    [RegistrationOptionsKey(nameof(ServerCapabilities.SubLensProvider))]
    public partial class SubLensRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        public TextDocumentSelector? DocumentSelector { get; set; }

        [Optional]
        public bool WorkDoneProgress { get; set; }

        [RegistrationOptionsKey(nameof(ServerCapabilities.SubLensProvider))]
        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            /// <summary>
            /// Code lens has a resolve provider as well.
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            [Optional]
            public bool WorkDoneProgress { get; set; }
        }
    }
}
#nullable restore
