using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization.Converters;
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
            GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Proposals"),
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

        /// <summary>
        /// Moniker uniqueness level to define scope of the moniker.
        /// </summary>
        [DebuggerDisplay("{" + nameof(_value) + "}")]
        [JsonConverter(typeof(EnumLikeStringConverter))]
        public readonly struct MonikerKind : IEquatable<MonikerKind>, IEnumLikeString
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

            private readonly string? _value;

            public MonikerKind(string kind) => _value = kind;

            public static implicit operator MonikerKind(string kind) => new MonikerKind(kind);

            public static implicit operator string(MonikerKind kind) => kind._value ?? string.Empty;

            /// <inheritdoc />
            public override string ToString() => _value ?? string.Empty;

            public bool Equals(MonikerKind other) => _value == other._value;

            public override bool Equals(object obj) => obj is MonikerKind other && Equals(other);

            public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

            public static bool operator ==(MonikerKind left, MonikerKind right) => left.Equals(right);

            public static bool operator !=(MonikerKind left, MonikerKind right) => !left.Equals(right);
        }


    /// <summary>
    /// A set of predefined code action kinds
    /// </summary>
    [DebuggerDisplay("{" + nameof(_value) + "}")]
    [JsonConverter(typeof(EnumLikeStringConverter))]
    public readonly struct UniquenessLevel : IEquatable<UniquenessLevel>, IEnumLikeString
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

        private readonly string? _value;

        public UniquenessLevel(string kind) => _value = kind;

        public static implicit operator UniquenessLevel(string kind) => new UniquenessLevel(kind);

        public static implicit operator string(UniquenessLevel kind) => kind._value ?? string.Empty;

        /// <inheritdoc />
        public override string ToString() => _value ?? string.Empty;

        public bool Equals(UniquenessLevel other) => _value == other._value;

        public override bool Equals(object obj) => obj is UniquenessLevel other && Equals(other);

        public override int GetHashCode() => _value != null ? _value.GetHashCode() : 0;

        public static bool operator ==(UniquenessLevel left, UniquenessLevel right) => left.Equals(right);

        public static bool operator !=(UniquenessLevel left, UniquenessLevel right) => !left.Equals(right);
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