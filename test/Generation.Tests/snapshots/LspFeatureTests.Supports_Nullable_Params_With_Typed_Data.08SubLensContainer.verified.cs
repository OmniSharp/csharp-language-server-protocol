//HintName: SubLensContainer.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Immutable;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test.Models
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial class SubLensContainer : ContainerBase<SubLens>
    {
        public SubLensContainer() : this(Enumerable.Empty<SubLens>())
        {
        }

        public SubLensContainer(IEnumerable<SubLens> items) : base(items)
        {
        }

        public SubLensContainer(params SubLens[] items) : base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer? From(IEnumerable<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer? (SubLens[] items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer? From(params SubLens[] items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer? (Collection<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer? From(Collection<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer? (List<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer? From(List<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer? (in ImmutableArray<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer? From(in ImmutableArray<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer? (ImmutableList<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer? From(ImmutableList<SubLens>? items) => items switch
        {
            not null => new SubLensContainer(items),
            _ => null
        };
    }
}
#nullable restore
