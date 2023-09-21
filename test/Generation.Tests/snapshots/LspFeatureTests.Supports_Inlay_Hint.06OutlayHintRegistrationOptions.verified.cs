//HintName: OutlayHintRegistrationOptions.cs
using System.Diagnostics;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test.Models
{
    [RegistrationOptionsKey(nameof(ServerCapabilities.OutlayHintProvider))]
    public partial class OutlayHintRegistrationOptions : OmniSharp.Extensions.LanguageServer.Protocol.IRegistrationOptions
    {
        public TextDocumentSelector? DocumentSelector { get; set; }

        [Optional]
        public bool WorkDoneProgress { get; set; }

        [RegistrationOptionsKey(nameof(ServerCapabilities.OutlayHintProvider))]
        public partial class StaticOptions : IWorkDoneProgressOptions
        {
            /// <summary>
            /// The server provides support to resolve additional
            /// information for a code action.
            ///
            /// @since 3.16.0
            /// </summary>
            [Optional]
            public bool ResolveProvider { get; set; }

            [Optional]
            public bool WorkDoneProgress { get; set; }
        }
    }
}
#nullable restore
