using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Obsolete(Constants.Proposal)]
        [Method(TextDocumentNames.OnTypeRename, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(OnTypeRenameRegistrationOptions)), Capability(typeof(OnTypeRenameClientCapabilities))]
        public partial class OnTypeRenameParams : TextDocumentPositionParams, IWorkDoneProgressParams, IRequest<OnTypeRenameRanges>
        {
        }

        [Obsolete(Constants.Proposal)]
        public partial class OnTypeRenameRanges
        {
            /// <summary>
            /// A list of ranges that can be renamed together. The ranges must have
            /// identical length and contain identical text content. The ranges cannot overlap.
            /// </summary>
            public Container<Range> Ranges { get; set; } = null!;

            /// <summary>
            /// An optional word pattern (regular expression) that describes valid contents for
            /// the given ranges. If no pattern is provided, the client configuration's word
            /// pattern will be used.
            /// </summary>
            [Optional]
            public string? WordPattern { get; set; }
        }

        [Obsolete(Constants.Proposal)]
        [GenerateRegistrationOptions(nameof(ServerCapabilities.OnTypeRenameProvider))]
        public partial class OnTypeRenameRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions { }

    }

    namespace Client.Capabilities
    {
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.OnTypeRename))]
        public partial class OnTypeRenameClientCapabilities : DynamicCapability, ConnectedCapability<IOnTypeRenameHandler> { }
    }

    namespace Document
    {
    }
}
