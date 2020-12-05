using System;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
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
        [Method(TextDocumentNames.Moniker, Direction.ClientToServer)]
        [
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals"),
            GenerateHandlerMethods,
            GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))
        ]
        [RegistrationOptions(typeof(MonikerRegistrationOptions)), Capability(typeof(MonikerCapability))]
        public partial record MonikerParams : TextDocumentPositionParams, IWorkDoneProgressParams, IPartialItemsRequest<Container<Moniker>?, Moniker>
        {
        }

        /// <summary>
        /// Moniker definition to match LSIF 0.5 moniker definition.
        /// </summary>
        [Obsolete(Constants.Proposal)]
        public partial record Moniker
        {
            /// <summary>
            /// The scheme of the moniker. For example tsc or .Net
            /// </summary>
            public string Scheme { get; init; }

            /// <summary>
            /// The identifier of the moniker. The value is opaque in LSIF however
            /// schema owners are allowed to define the structure if they want.
            /// </summary>
            public string Identifier { get; init; }

            /// <summary>
            /// The scope in which the moniker is unique
            /// </summary>
            public UniquenessLevel Unique { get; init; }

            /// <summary>
            /// The moniker kind if known.
            /// </summary>
            [Optional]
            public MonikerKind Kind { get; init; }
        }

        /// <summary>
        /// Moniker uniqueness level to define scope of the moniker.
        /// </summary>
        [StringEnum]
        public readonly partial struct MonikerKind
        {
            /// <summary>
            /// The moniker represent a symbol that is imported into a project
            /// </summary>
            public static readonly MonikerKind Import = new MonikerKind("import");

            /// <summary>
            /// The moniker represents a symbol that is exported from a project
            /// </summary>
            public static readonly MonikerKind Export = new MonikerKind("export");

            /// <summary>
            /// The moniker represents a symbol that is local to a project (e.g. a local
            /// variable of a function, a class not visible outside the project, ...)
            /// </summary>
            public static readonly MonikerKind Local = new MonikerKind("local");
        }


        /// <summary>
        /// A set of predefined code action kinds
        /// </summary>
        [StringEnum]
        public readonly partial struct UniquenessLevel
        {
            /// <summary>
            /// The moniker is only unique inside a document
            /// </summary>
            public static readonly UniquenessLevel Document = new UniquenessLevel("document");

            /// <summary>
            /// The moniker is unique inside a project for which a dump got created
            /// </summary>
            public static readonly UniquenessLevel Project = new UniquenessLevel("project");

            /// <summary>
            /// The moniker is unique inside the group to which a project belongs
            /// </summary>
            public static readonly UniquenessLevel Group = new UniquenessLevel("group");

            /// <summary>
            /// The moniker is unique inside the moniker scheme.
            /// </summary>
            public static readonly UniquenessLevel Scheme = new UniquenessLevel("scheme");

            /// <summary>
            /// The moniker is globally unique
            /// </summary>
            public static readonly UniquenessLevel Global = new UniquenessLevel("global");
        }

        [Obsolete(Constants.Proposal)]
        [GenerateRegistrationOptions(nameof(ServerCapabilities.MonikerProvider))]
        [RegistrationName(TextDocumentNames.Moniker)]
        public partial class MonikerRegistrationOptions : ITextDocumentRegistrationOptions, IWorkDoneProgressOptions
        {
        }
    }

    namespace Client.Capabilities
    {
        [Obsolete(Constants.Proposal)]
        [CapabilityKey(nameof(ClientCapabilities.TextDocument), nameof(TextDocumentClientCapabilities.Moniker))]
        public partial class MonikerCapability : DynamicCapability
        {
        }
    }

    namespace Document.Proposals
    {
    }
}
