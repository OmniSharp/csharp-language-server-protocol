using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;

namespace OmniSharp.Extensions.LanguageServer.Protocol.Models.Proposals
{
    /// <summary>
    /// Represents programming constructs like functions or constructors in the context
    /// of call hierarchy.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class CallHierarchyItem : ICanBeResolved
    {
        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The kind of this item.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// Tags for this item.
        /// </summary>
        [Optional]
        public Container<SymbolTag>? Tags { get; set; }

        /// <summary>
        /// More detail for this item, e.g. the signature of a function.
        /// </summary>
        [Optional]
        public string? Detail { get; set; }

        /// <summary>
        /// The resource identifier of this item.
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        /// <summary>
        /// The range enclosing this symbol not including leading/trailing whitespace but everything else, e.g. comments and code.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
        /// Must be contained by the [`range`](#CallHierarchyItem.range).
        /// </summary>
        public Range SelectionRange { get; set; } = null!;

        /// <summary>
        /// A data entry field that is preserved between a call hierarchy prepare and
        /// incoming calls or outgoing calls requests.
        /// </summary>
        [Optional]
        public JToken? Data { get; set; }

        private string DebuggerDisplay =>
            $"[{Kind.ToString()}] " +
            $"{Name} " +
            $"@ {Uri} " +
            $"{Range}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }

    /// <summary>
    /// Represents programming constructs like functions or constructors in the context
    /// of call hierarchy.
    ///
    /// @since 3.16.0
    /// </summary>
    [Obsolete(Constants.Proposal)]
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class CallHierarchyItem<T> : ICanBeResolved
        where T : HandlerIdentity?, new()
    {
        /// <summary>
        /// The name of this item.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// The kind of this item.
        /// </summary>
        public SymbolKind Kind { get; set; }

        /// <summary>
        /// Tags for this item.
        /// </summary>
        [Optional]
        public Container<SymbolTag>? Tags { get; set; }

        /// <summary>
        /// More detail for this item, e.g. the signature of a function.
        /// </summary>
        [Optional]
        public string? Detail { get; set; }

        /// <summary>
        /// The resource identifier of this item.
        /// </summary>
        public DocumentUri Uri { get; set; } = null!;

        /// <summary>
        /// The range enclosing this symbol not including leading/trailing whitespace but everything else, e.g. comments and code.
        /// </summary>
        public Range Range { get; set; } = null!;

        /// <summary>
        /// The range that should be selected and revealed when this symbol is being picked, e.g. the name of a function.
        /// Must be contained by the [`range`](#CallHierarchyItem.range).
        /// </summary>
        public Range SelectionRange { get; set; } = null!;

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        public T Data
        {
            get => ( (ICanBeResolved) this ).Data?.ToObject<T>()!;
            set => ( (ICanBeResolved) this ).Data = JToken.FromObject(value);
        }

        JToken? ICanBeResolved.Data { get; set; }

        public static implicit operator CallHierarchyItem(CallHierarchyItem<T> value) => new CallHierarchyItem {
            Data = ( (ICanBeResolved) value ).Data,
            Detail = value.Detail,
            Kind = value.Kind,
            Name = value.Name,
            Range = value.Range,
            SelectionRange = value.SelectionRange,
            Tags = value.Tags,
            Uri = value.Uri
        };

        public static implicit operator CallHierarchyItem<T>(CallHierarchyItem value)
        {
            var item = new CallHierarchyItem<T> {
                Detail = value.Detail,
                Kind = value.Kind,
                Name = value.Name,
                Range = value.Range,
                SelectionRange = value.SelectionRange,
                Tags = value.Tags,
                Uri = value.Uri
            };
            ( (ICanBeResolved) item ).Data = value.Data;
            return item;
        }

        private string DebuggerDisplay =>
            $"[{Kind.ToString()}] " +
            $"{Name} " +
            $"@ {Uri} " +
            $"{Range}";

        /// <inheritdoc />
        public override string ToString() => DebuggerDisplay;
    }
}
