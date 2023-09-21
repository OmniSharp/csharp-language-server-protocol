//HintName: Test0_OutlayHintTyped.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test.Models
{
    public partial record OutlayHint
    {
        public OutlayHint<TData> WithData<TData>(TData data)
            where TData : class?, IHandlerIdentity?
        {
            return new OutlayHint<TData>{Position = Position, Label = Label, Kind = Kind, TextEdits = TextEdits, Tooltip = Tooltip, PaddingLeft = PaddingLeft, PaddingRight = PaddingRight, Data = data};
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
        public static OutlayHint? From<T>(OutlayHint<T>? item)
            where T : class?, IHandlerIdentity? => item switch
        {
            not null => item,
            _ => null
        };
    }

    /// <summary>
    /// Outlay hint information.
    ///
    /// @since 3.17.0
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [Parallel]
    [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document", Name = "OutlayHintResolve")]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    [GenerateContainer]
    [Capability(typeof(OutlayHintWorkspaceClientCapabilities))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial record OutlayHint<T> : ICanBeResolved where T : class?, IHandlerIdentity?
    {
        /// <summary>
        /// The position of this hint.
        /// </summary>
        public Position Position { get; init; }

        /// <summary>
        /// The label of this hint. A human readable string or an array of
        /// OutlayHintLabelPart label parts.
        ///
        /// *Note* that neither the string nor the label part can be empty.
        /// </summary>
        public StringOrOutlayHintLabelParts Label { get; init; }

        /// <summary>
        /// The kind of this hint. Can be omitted in which case the client
        /// should fall back to a reasonable default.
        /// </summary>
        public OutlayHintKind? Kind { get; init; }

        /// <summary>
        /// Optional text edits that are performed when accepting this inlay hint.
        ///
        /// *Note* that edits are expected to change the document so that the inlay
        /// hint (or its nearest variant) is now part of the document and the inlay
        /// hint itself is now obsolete.
        ///
        /// Depending on the client capability `inlayHint.resolveSupport` clients
        /// might resolve this property late using the resolve request.
        /// </summary>
        [Optional]
        public Container<TextEdit>? TextEdits { get; init; }

        /// <summary>
        /// The tooltip text when you hover over this item.
        ///
        /// Depending on the client capability `inlayHint.resolveSupport` clients
        /// might resolve this property late using the resolve request.
        /// </summary>
        [Optional]
        public StringOrMarkupContent? Tooltip { get; init; }

        /// <summary>
        /// Render padding before the hint.
        ///
        /// Note: Padding should use the editor's background color, not the
        /// background color of the hint itself. That means padding can be used
        /// to visually align/separate an inlay hint.
        /// </summary>
        [Optional]
        public bool? PaddingLeft { get; init; }

        /// <summary>
        /// Render padding after the hint.
        ///
        /// Note: Padding should use the editor's background color, not the
        /// background color of the hint itself. That means padding can be used
        /// to visually align/separate an inlay hint.
        /// </summary>
        [Optional]
        public bool? PaddingRight { get; init; }

        /// <summary>
        /// A data entry field that is preserved on a document link between a
        /// DocumentLinkRequest and a DocumentLinkResolveRequest.
        /// </summary>
        [Optional]
        public T Data { get => this.GetRawData<T>()!; init => this.SetRawData<T>(value); }

        private string DebuggerDisplay => ToString();
        public OutlayHint<TData> WithData<TData>(TData data)
            where TData : class?, IHandlerIdentity?
        {
            return new OutlayHint<TData>{Position = Position, Label = Label, Kind = Kind, TextEdits = TextEdits, Tooltip = Tooltip, PaddingLeft = PaddingLeft, PaddingRight = PaddingRight, Data = data};
        }

        JToken? ICanBeResolved.Data { get; init; }

        private JToken? JData { get => this.GetRawData(); init => this.SetRawData(value); }

        public static implicit operator OutlayHint<T>(OutlayHint value) => new OutlayHint<T>{Position = value.Position, Label = value.Label, Kind = value.Kind, TextEdits = value.TextEdits, Tooltip = value.Tooltip, PaddingLeft = value.PaddingLeft, PaddingRight = value.PaddingRight, JData = value.Data};
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
        public static implicit operator OutlayHint? (OutlayHint<T>? value) => value switch
        {
            not null => new OutlayHint{Position = value.Position, Label = value.Label, Kind = value.Kind, TextEdits = value.TextEdits, Tooltip = value.Tooltip, PaddingLeft = value.PaddingLeft, PaddingRight = value.PaddingRight, Data = value.JData},
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
        public static OutlayHint<T>? From(OutlayHint? item) => item switch
        {
            not null => item,
            _ => null
        };
    }

    public partial class OutlayHintContainer<T> : ContainerBase<OutlayHint<T>> where T : class?, IHandlerIdentity?
    {
        public OutlayHintContainer() : this(Enumerable.Empty<OutlayHint<T>>())
        {
        }

        public OutlayHintContainer(IEnumerable<OutlayHint<T>> items) : base(items)
        {
        }

        public OutlayHintContainer(params OutlayHint<T>[] items) : base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer<T>? From(IEnumerable<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer<T>? (OutlayHint<T>[] items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer<T>? From(params OutlayHint<T>[] items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer<T>? (Collection<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer<T>? From(Collection<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer<T>? (List<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer<T>? From(List<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer<T>? (in ImmutableArray<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer<T>? From(in ImmutableArray<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer<T>? (ImmutableList<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer<T>? From(ImmutableList<OutlayHint<T>>? items) => items switch
        {
            not null => new OutlayHintContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("container")]
        public static implicit operator OutlayHintContainer? (OutlayHintContainer<T>? container) => container switch
        {
            not null => new OutlayHintContainer(container.Select(value => (OutlayHint)value)),
            _ => null
        };
    }
}
#nullable restore
