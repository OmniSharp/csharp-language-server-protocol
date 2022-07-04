//HintName: OutlayHintContainer.cs
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
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial class OutlayHintContainer : ContainerBase<OutlayHint>
    {
        public OutlayHintContainer() : this(Enumerable.Empty<OutlayHint>())
        {
        }

        public OutlayHintContainer(IEnumerable<OutlayHint> items) : base(items)
        {
        }

        public OutlayHintContainer(params OutlayHint[] items) : base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer? From(IEnumerable<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer? (OutlayHint[] items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer? From(params OutlayHint[] items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer? (Collection<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer? From(Collection<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer? (List<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer? From(List<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer? (in ImmutableArray<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer? From(in ImmutableArray<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator OutlayHintContainer? (ImmutableList<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static OutlayHintContainer? From(ImmutableList<OutlayHint>? items) => items switch
        {
            not null => new OutlayHintContainer(items),
            _ => null
        };
    }
}
#nullable restore
