using System;
using MediatR;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models
    {
        [Parallel]
        [Obsolete(Constants.Proposal)]
        [Method(TextDocumentNames.LinkedEditingRange, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(LinkedEditingRangeRegistrationOptions)), Capability(typeof(LinkedEditingRangeClientCapabilities))]
        public partial record LinkedEditingRangeParams : TextDocumentPositionParams, IWorkDoneProgressParams, IRequest<LinkedEditingRanges>
        {
        }

        [Obsolete(Constants.Proposal)]
        public partial record LinkedEditingRanges
        {
            /// <summary>
            /// A list of ranges that can be renamed together. The ranges must have
            /// identical length and contain identical text content. The ranges cannot overlap.
            /// </summary>
            public Container<Range> Ranges { get; init; }

            /// <summary>
            /// An optional word pattern (regular expression) that describes valid contents for
            /// the given ranges. If no pattern is provided, the client configuration's word
            /// pattern will be used.
            /// </summary>
            [Optional]
            public string? WordPattern { get; init; }
        }

        [Obsolete(Constants.Proposal)]
        [GenerateRegistrationOptions(nameof(ServerCapabilities.LinkedEditingRangeProvider))]
        [RegistrationName(TextDocumentNames.LinkedEditingRange)]
        public partial class LinkedEditingRangeRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions { }

    }

    namespace Client.Capabilities
    {
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.LinkedEditingRange))]
        public partial class LinkedEditingRangeClientCapabilities : DynamicCapability { }
    }

    namespace Document
    {
    }
}
