//HintName: CodeLensContainer.cs
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.JsonRpc.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial class CodeLensContainer : ContainerBase<CodeLens>
    {
        public CodeLensContainer() : this(Enumerable.Empty<CodeLens>())
        {
        }

        public CodeLensContainer(IEnumerable<CodeLens> items) : base(items)
        {
        }

        public CodeLensContainer(params CodeLens[] items) : base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static CodeLensContainer? From(IEnumerable<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator CodeLensContainer? (CodeLens[] items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static CodeLensContainer? From(params CodeLens[] items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator CodeLensContainer? (Collection<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static CodeLensContainer? From(Collection<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator CodeLensContainer? (List<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static CodeLensContainer? From(List<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator CodeLensContainer? (in ImmutableArray<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static CodeLensContainer? From(in ImmutableArray<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator CodeLensContainer? (ImmutableList<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static CodeLensContainer? From(ImmutableList<CodeLens>? items) => items switch
        {
            not null => new CodeLensContainer(items),
            _ => null
        };
    }
}
#nullable restore
