using System;
using System.Threading;
using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;

// ReSharper disable once CheckNamespace
namespace OmniSharp.Extensions.LanguageServer.Protocol
{
    namespace Models.Proposals
    {
        /// <summary>
        /// Language Server Index Format (LSIF) introduced the concept of symbol monikers to help associate symbols across different indexes.
        /// This request adds capability for LSP server implementations to provide the same symbol moniker information given a text document
        /// position. Clients can utilize this method to get the moniker at the current location in a file user is editing and do further
        /// code navigation queries in other services that rely on LSIF indexes and link symbols together.
        ///
        /// The `textDocument/moniker` request is sent from the client to the server to get the symbol monikers for a given text document
        /// position. An array of Moniker types is returned as response to indicate possible monikers at the given location. If no monikers
        /// can be calculated, an empty array or `null` should be returned.
        /// </summary>
        [Obsolete(Constants.Proposal)]
        [Parallel]
        [Method(TextDocumentNames.Moniker, Direction.ClientToServer)][
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(MonikerRegistrationOptions)), Capability(typeof(MonikerCapability))]
        public partial class MonikerParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<Container<Moniker>?, Moniker>
        {
        }

        /// <summary>
        /// Moniker definition to match LSIF 0.5 moniker definition.
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial class Moniker
        {
            /// <summary>
            /// The scheme of the moniker. For example tsc or .Net
            /// </summary>
            public string Scheme { get; set; } = null!;

            /// <summary>
            /// The identifier of the moniker. The value is opaque in LSIF however
            /// schema owners are allowed to define the structure if they want.
            /// </summary>
            public string Identifier { get; set; } = null!;

            /// <summary>
            /// The scope in which the moniker is unique
            /// </summary>
            public UniquenessLevel Unique { get; set; }

            /// <summary>
            /// The moniker kind if known.
            /// </summary>
            [Optional]
            public MonikerKind Kind { get; set; }
        }

        [Obsolete(Constants.Proposal)]
        [GenerateRegistrationOptions(nameof(ServerCapabilities.MonikerProvider))]
        public partial class MonikerRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions { }

    }

    namespace Client.Capabilities
    {
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Moniker))]
        public partial class MonikerCapability : DynamicCapability { }
    }

    namespace Document.Proposals
    {

    }
}
