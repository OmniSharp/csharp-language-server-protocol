using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using TestingUtils;

namespace Generation.Tests
{
    public class TypedCanBeResolvedTests
    {
        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Generating_Strongly_Typed_ICanBeResolved_Data()
        {
            var source = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    [DebuggerDisplay(""{"" + nameof(DebuggerDisplay) + "",nq}"")]
    [Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    [GenerateTypedData, GenerateContainer]
    public partial class CodeLens : IRequest<CodeLens>, ICanBeResolved
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; set; } = null!;
        [Optional]
        public Command? Command { get; set; }
        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public JToken? Data { get; set; }
        private string DebuggerDisplay => $""{Range}{( Command != null ? $"" {Command}"" : """" )}"";
        public override string ToString() => DebuggerDisplay;
    }
}
#nullable restore";

            var expected = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Immutable;
using System.Linq;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    public partial class CodeLens
    {
        public CodeLens<TData> WithData<TData>(TData data)
            where TData : HandlerIdentity? , new()
        {
            return new CodeLens<TData>{Range = Range, Command = Command, Data = data};
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""item"")]
        public static CodeLens? From<T>(CodeLens<T>? item)
            where T : HandlerIdentity? , new() => item switch
        {
        not null => item, _ => null
        }

        ;
    }

    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    [DebuggerDisplay(""{"" + nameof(DebuggerDisplay) + "",nq}"")]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    public partial class CodeLens<T> : ICanBeResolved where T : HandlerIdentity? , new()
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range
        {
            get;
            set;
        }

        = null !;
        [Optional]
        public Command? Command
        {
            get;
            set;
        }

        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public T Data
        {
            get => ((ICanBeResolved)this).Data?.ToObject<T>()!;
            set => ((ICanBeResolved)this).Data = JToken.FromObject(value);
        }

        private string DebuggerDisplay => $""{Range}{(Command != null ? $"" {Command}"" : """")}"";
        public override string ToString() => DebuggerDisplay;
        public CodeLens<TData> WithData<TData>(TData data)
            where TData : HandlerIdentity? , new()
        {
            return new CodeLens<TData>{Range = Range, Command = Command, Data = data};
        }

        JToken? ICanBeResolved.Data
        {
            get;
            set;
        }

        private JToken? JData
        {
            get => ((ICanBeResolved)this).Data;
            set => ((ICanBeResolved)this).Data = value;
        }

        public static implicit operator CodeLens<T>(CodeLens value) => new CodeLens<T>{Range = value.Range, Command = value.Command, JData = ((ICanBeResolved)value).Data};
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""value"")]
        public static implicit operator CodeLens? (CodeLens<T>? value) => value switch
        {
        not null => new CodeLens{Range = value.Range, Command = value.Command, Data = ((ICanBeResolved)value).Data}, _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""item"")]
        public static CodeLens<T>? From(CodeLens? item) => item switch
        {
        not null => item, _ => null
        }

        ;
    }

    public partial class CodeLensContainer<T> : ContainerBase<CodeLens<T>> where T : HandlerIdentity? , new()
    {
        public CodeLensContainer(): this(Enumerable.Empty<CodeLens<T>>())
        {
        }

        public CodeLensContainer(IEnumerable<CodeLens<T>> items): base(items)
        {
        }

        public CodeLensContainer(params CodeLens<T>[] items): base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer<T>? From(IEnumerable<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer<T>? (CodeLens<T>[] items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer<T>? From(params CodeLens<T>[] items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer<T>? (Collection<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer<T>? From(Collection<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer<T>? (List<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer<T>? From(List<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer<T>? (in ImmutableArray<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer<T>? From(in ImmutableArray<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer<T>? (ImmutableList<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer<T>? From(ImmutableList<CodeLens<T>>? items) => items switch
        {
        not null => new CodeLensContainer<T>(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""container"")]
        public static implicit operator CodeLensContainer? (CodeLensContainer<T>? container) => container switch
        {
        not null => new CodeLensContainer(container.Select(value => (CodeLens)value)), _ => null
        }

        ;
    }
}
#nullable restore

";
            await GenerationHelpers.AssertGeneratedAsExpected<StronglyTypedGenerator>(source, expected);
        }

        [FactWithSkipOn(SkipOnPlatform.Windows)]
        public async Task Supports_Generating_Strongly_Typed_Container()
        {
            var source = @"
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

#nullable enable
namespace OmniSharp.Extensions.LanguageServer.Protocol.Test
{
    /// <summary>
    /// A code lens represents a command that should be shown along with
    /// source text, like the number of references, a way to run tests, etc.
    ///
    /// A code lens is _unresolved_ when no command is associated to it. For performance
    /// reasons the creation of a code lens and resolving should be done in two stages.
    /// </summary>
    [DebuggerDisplay(""{"" + nameof(DebuggerDisplay) + "",nq}"")]
    [Method(TextDocumentNames.CodeLensResolve, Direction.ClientToServer)]
    [GenerateContainer]
    public partial class CodeLens : IRequest<CodeLens>, ICanBeResolved
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; set; } = null!;
        [Optional]
        public Command? Command { get; set; }
        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public JToken? Data { get; set; }
        private string DebuggerDisplay => $""{Range}{( Command != null ? $"" {Command}"" : """" )}"";
        public override string ToString() => DebuggerDisplay;
    }
}
#nullable restore";

            var expected = @"using System;
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
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
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
        public CodeLensContainer(): this(Enumerable.Empty<CodeLens>())
        {
        }

        public CodeLensContainer(IEnumerable<CodeLens> items): base(items)
        {
        }

        public CodeLensContainer(params CodeLens[] items): base(items)
        {
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer? From(IEnumerable<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer? (CodeLens[] items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer? From(params CodeLens[] items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer? (Collection<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer? From(Collection<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer? (List<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer? From(List<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer? (in ImmutableArray<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer? From(in ImmutableArray<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static implicit operator CodeLensContainer? (ImmutableList<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull(""items"")]
        public static CodeLensContainer? From(ImmutableList<CodeLens>? items) => items switch
        {
        not null => new CodeLensContainer(items), _ => null
        }

        ;
    }
}
#nullable restore";
            await GenerationHelpers.AssertGeneratedAsExpected<StronglyTypedGenerator>(source, expected);
        }
    }
}
