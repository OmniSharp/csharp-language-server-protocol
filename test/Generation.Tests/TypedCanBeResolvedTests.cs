using System.Threading.Tasks;
using OmniSharp.Extensions.JsonRpc.Generators;
using Xunit;

namespace Generation.Tests
{
    [UsesVerify]
    public class TypedCanBeResolvedTests
    {
        [Fact]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

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
    public partial record CodeLens : IRequest<CodeLens>, ICanBeResolved
    {
        /// <summary>
        /// The range in which this code lens is valid. Should only span a single line.
        /// </summary>
        public Range Range { get; init; }
        [Optional]
        public Command? Command { get; init; }
        /// <summary>
        /// A data entry field that is preserved on a code lens item between
        /// a code lens and a code lens resolve request.
        /// </summary>
        [Optional]
        public JToken? Data { get; init; }
        private string DebuggerDisplay => $""{Range}{( Command != null ? $"" {Command}"" : """" )}"";
        public override string ToString() => DebuggerDisplay;
    }
}
#nullable restore";

            await Verify(GenerationHelpers.GenerateAll(source));
        }

        [Fact]
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
using OmniSharp.Extensions.LanguageServer.Protocol.Generation;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Serialization;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;

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
        public JToken? Data { get; init; }
        private string DebuggerDisplay => $""{Range}{( Command != null ? $"" {Command}"" : """" )}"";
        public override string ToString() => DebuggerDisplay;
    }
}
#nullable restore";

            await Verify(GenerationHelpers.GenerateAll(source));
        }
    }
}
