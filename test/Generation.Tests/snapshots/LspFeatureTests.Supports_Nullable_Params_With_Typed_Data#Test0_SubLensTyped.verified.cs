//HintName: Test0_SubLensTyped.cs
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
    public partial record SubLens
    {
        public SubLens<TData> WithData<TData>(TData data)
            where TData : class?, IHandlerIdentity?
        {
            return new SubLens<TData>
            {
                Range = Range,
                Command = Command,
                Data = data
            };
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
        public static SubLens? From<T>(SubLens<T>? item)
            where T : class?, IHandlerIdentity? => item switch
        {
            not null => item,
            _ => null
        };
    }

    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [Parallel]
    [GenerateHandler("OmniSharp.Extensions.LanguageServer.Protocol.Document.Test", Name = "SubLensResolve")]
    [GenerateHandlerMethods]
    [GenerateRequestMethods(typeof(ITextDocumentLanguageClient), typeof(ILanguageClient))]
    [GenerateContainer]
    [Capability(typeof(SubLensCapability))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial record SubLens<T> : ICanBeResolved where T : class?, IHandlerIdentity?
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; init; } = null !;

        /// <summary>
        /// The command this code lens represents.
        /// </summary>
        [Optional]
        public Command? Command { get; init; }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public T Data { get => this.GetRawData<T>()!; init => this.SetRawData<T>(value); }
        private string DebuggerDisplay => $"{Range}{(Command != null ? $" {Command}" : "")}";

        /// <inheritdoc/>
        public override string ToString()
        {
            return DebuggerDisplay;
        }

        public SubLens<TData> WithData<TData>(TData data)
            where TData : class?, IHandlerIdentity?
        {
            return new SubLens<TData>
            {
                Range = Range,
                Command = Command,
                Data = data
            };
        }

        JToken? ICanBeResolved.Data { get; init; }
        private JToken? JData { get => this.GetRawData(); init => this.SetRawData(value); }

        public static implicit operator SubLens<T>(SubLens value) => new SubLens<T>
        {
            Range = value.Range,
            Command = value.Command,
            JData = value.Data
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("value")]
        public static implicit operator SubLens? (SubLens<T>? value) => value switch
        {
            not null => new SubLens
            {
                Range = value.Range,
                Command = value.Command,
                Data = value.JData
            },
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")]
        public static SubLens<T>? From(SubLens? item) => item switch
        {
            not null => item,
            _ => null
        };
    }

    public partial class SubLensContainer<T> : ContainerBase<SubLens<T>> where T : class?, IHandlerIdentity?
    {
        public SubLensContainer() : this(Enumerable.Empty<SubLens<T>>())
        {
        }

        public SubLensContainer(IEnumerable<SubLens<T>> items) : base(items)
        {
        }

        public SubLensContainer(params SubLens<T>[] items) : base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer<T>? From(IEnumerable<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer<T>? (SubLens<T>[] items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer<T>? From(params SubLens<T>[] items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer<T>? (Collection<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer<T>? From(Collection<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer<T>? (List<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer<T>? From(List<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer<T>? (in ImmutableArray<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer<T>? From(in ImmutableArray<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static implicit operator SubLensContainer<T>? (ImmutableList<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("items")]
        public static SubLensContainer<T>? From(ImmutableList<SubLens<T>>? items) => items switch
        {
            not null => new SubLensContainer<T>(items),
            _ => null
        };
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("container")]
        public static implicit operator SubLensContainer? (SubLensContainer<T>? container) => container switch
        {
            not null => new SubLensContainer(container.Select(value => (SubLens)value)),
            _ => null
        };
    }
}
#nullable restore
